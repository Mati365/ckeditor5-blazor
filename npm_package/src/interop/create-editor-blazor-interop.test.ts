import type { DotNetInterop } from '../types';
import type { Mock } from 'vitest';

import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import {
  createDotnet,
  createDotNetInteropMock,
  DEFAULT_TEST_EDITOR_ID,
  renderTestEditor,
  waitForDestroyAllEditors,
  waitForTestEditor,
} from '~/test-utils';

import { timeout } from '../../src/shared';
import { CKEditor5ChangeDataEvent } from '../elements/editor/plugins/dispatch-editor-roots-change-event';
import { ensureEditorElementsRegistered } from '../elements/ensure-editor-elements-registered';
import { createEditorBlazorInterop } from './create-editor-blazor-interop';

describe('createEditorBlazorInterop', () => {
  let element: HTMLElement;
  let dotnetInterop: DotNetInterop;

  beforeEach(() => {
    document.body.innerHTML = '';
    ensureEditorElementsRegistered();

    dotnetInterop = createDotNetInteropMock();
    globalThis.DotNet = createDotnet();

    element = renderTestEditor({}, {
      interactive: false,
    });
  });

  afterEach(async () => {
    vi.useRealTimers();
    vi.resetAllMocks();

    globalThis.DotNet = undefined as unknown as typeof DotNet;
    document.body.innerHTML = '';

    await waitForDestroyAllEditors();
  });

  it('sets [data-cke-interactive=true] attribute on the element', async () => {
    expect(element.hasAttribute('data-cke-interactive')).toBe(false);

    createEditorBlazorInterop(element, dotnetInterop);

    const editor = await waitForTestEditor();

    expect(editor).toBeDefined();
    expect(element.getAttribute('data-cke-interactive')).toBe('true');
  });

  it('should ignore ckeditor5 change events from other editors', async () => {
    createEditorBlazorInterop(element, dotnetInterop);

    const editor = await waitForTestEditor();

    const changeEvent = new CKEditor5ChangeDataEvent({
      editorId: 'other-editor',
      editor,
      roots: { main: 'changed' },
    });

    document.body.dispatchEvent(changeEvent);

    expect(dotnetInterop.invokeMethodAsync).not.toHaveBeenCalledWith(
      'OnChangeEditorData',
      expect.anything(),
      expect.anything(),
    );
  });

  describe('setValue', () => {
    it('should be possible to call `setValue` while editor is initializing', async () => {
      const { setValue } = createEditorBlazorInterop(element, dotnetInterop);

      expect(() => setValue({ main: 'test' })).not.toThrow();

      const editor = await waitForTestEditor();

      expect(editor.getData()).toBe('<p>test</p>');
    });

    it('should not set data if the interop is unmounted before the editor is ready', async () => {
      const { setValue, unmount } = createEditorBlazorInterop(element, dotnetInterop);

      unmount();

      expect(() => setValue({ main: 'test' })).not.toThrow();

      const editor = await waitForTestEditor();

      expect(editor.getData()).toBe('<p>Initial content</p>');
    });

    it('should delay setting data if the editor is focused', async () => {
      const { setValue } = createEditorBlazorInterop(element, dotnetInterop);
      const editor = await waitForTestEditor();

      editor.ui.focusTracker.isFocused = true;

      await setValue({ main: 'focused update' });

      expect(editor.getData()).toBe('<p>Initial content</p>');

      editor.ui.focusTracker.isFocused = false;

      await vi.waitFor(() => expect(editor.getData()).toBe('<p>focused update</p>'));
    });
  });

  describe('setRootAttributes', () => {
    it('should update root attributes on the editor', async () => {
      const { setRootAttributes } = createEditorBlazorInterop(element, dotnetInterop);

      const editor = await waitForTestEditor();
      const root = editor.model.document.getRoot()!;

      expect(root.getAttribute('data-test')).toBeUndefined();

      await setRootAttributes({ 'data-test': 'value' });
      expect(root.getAttribute('data-test')).toBe('value');
    });

    it('should only remove attributes that it previously set', async () => {
      const { setRootAttributes } = createEditorBlazorInterop(element, dotnetInterop);

      const editor = await waitForTestEditor();
      const root = editor.model.document.getRoot()!;

      // Simulate another consumer setting an attribute.
      editor.model.change(writer => writer.setAttribute('data-keep', 'true', root));
      expect(root.getAttribute('data-keep')).toBe('true');

      await setRootAttributes({ 'data-test': 'value' });
      expect(root.getAttribute('data-test')).toBe('value');
      expect(root.getAttribute('data-keep')).toBe('true');

      // Clearing should remove only the attribute managed by us.
      await setRootAttributes(null);

      expect(root.getAttribute('data-test')).toBeUndefined();
      expect(root.getAttribute('data-keep')).toBe('true');
    });

    it('should not set root attributes if the interop is unmounted', async () => {
      const { setRootAttributes, unmount } = createEditorBlazorInterop(element, dotnetInterop);

      unmount();

      const editor = await waitForTestEditor();
      const root = editor.model.document.getRoot()!;

      await setRootAttributes({ 'data-test': 'value' });

      expect(root.getAttribute('data-test')).toBeUndefined();
    });
  });

  describe('focus tracking', () => {
    it('should call OnEditorFocus if editor gets focused', async () => {
      createEditorBlazorInterop(element, dotnetInterop);

      const editor = await waitForTestEditor();

      expect(dotnetInterop.invokeMethodAsync).not.toHaveBeenCalledWith('OnEditorFocus', expect.anything());

      editor.ui.focusTracker.isFocused = true;

      expect(dotnetInterop.invokeMethodAsync).toHaveBeenCalledWith('OnEditorFocus', expect.anything());
    });

    it('should call OnEditorBlur if editor gets blurred', async () => {
      createEditorBlazorInterop(element, dotnetInterop);

      const editor = await waitForTestEditor();

      expect(dotnetInterop.invokeMethodAsync).not.toHaveBeenCalledWith('OnEditorBlur', expect.anything());

      editor.ui.focusTracker.isFocused = true;

      await timeout(0);

      editor.ui.focusTracker.isFocused = false;

      await vi.waitFor(() => expect(dotnetInterop.invokeMethodAsync).toHaveBeenCalledWith('OnEditorBlur', expect.anything()));
    });
  });

  describe('unmount', () => {
    it('should remove event listeners and prevent future updates', async () => {
      const interop = createEditorBlazorInterop(element, dotnetInterop);
      const editor = await waitForTestEditor();

      const changeEvent = new CKEditor5ChangeDataEvent({
        editorId: DEFAULT_TEST_EDITOR_ID,
        editor,
        roots: { main: 'changed' },
      });

      document.body.dispatchEvent(changeEvent);

      expect(dotnetInterop.invokeMethodAsync).toHaveBeenCalledWith(
        'OnChangeEditorData',
        expect.anything(),
        { main: 'changed' },
      );

      (dotnetInterop.invokeMethodAsync as Mock).mockClear();

      interop.unmount();
      document.body.dispatchEvent(changeEvent);

      expect(dotnetInterop.invokeMethodAsync).not.toBeCalled();
    });

    it('should not crash if called twice', async () => {
      const interop = createEditorBlazorInterop(element, dotnetInterop);
      await waitForTestEditor();

      expect(() => {
        interop.unmount();
        interop.unmount();
      }).not.toThrow();
    });
  });

  describe('attachImageUploadAdapter', () => {
    it('should do nothing when already unmounted', async () => {
      const interop = createEditorBlazorInterop(element, dotnetInterop);

      await waitForTestEditor();
      interop.unmount();

      await expect(interop.attachImageUploadAdapter()).resolves.toBeUndefined();
      expect(dotnetInterop.invokeMethodAsync).not.toHaveBeenCalledWith(
        'OnEditorImageUpload',
        expect.anything(),
      );
    });

    it('should do nothing when FileRepository plugin is not available', async () => {
      const { attachImageUploadAdapter } = createEditorBlazorInterop(element, dotnetInterop);
      const editor = await waitForTestEditor();

      expect(editor.plugins.has('FileRepository')).toBe(false);

      await expect(attachImageUploadAdapter()).resolves.toBeUndefined();
    });

    it('should set createUploadAdapter on FileRepository when plugin is available', async () => {
      const { attachImageUploadAdapter } = createEditorBlazorInterop(element, dotnetInterop);
      const editor = await waitForTestEditor();

      const mockFileRepository: { createUploadAdapter: any; } = { createUploadAdapter: null };

      vi.spyOn(editor.plugins, 'has').mockReturnValue(true);
      vi.spyOn(editor.plugins, 'get').mockReturnValue(mockFileRepository as any);

      await attachImageUploadAdapter();

      expect(mockFileRepository.createUploadAdapter).toBeTypeOf('function');
    });

    it('upload adapter should call OnEditorImageUpload with file details and return the url', async () => {
      const { attachImageUploadAdapter } = createEditorBlazorInterop(element, dotnetInterop);
      const editor = await waitForTestEditor();

      const mockFileRepository: { createUploadAdapter: any; } = { createUploadAdapter: null };

      vi.spyOn(editor.plugins, 'has').mockReturnValue(true);
      vi.spyOn(editor.plugins, 'get').mockReturnValue(mockFileRepository as any);

      await attachImageUploadAdapter();

      const expectedUrl = 'https://example.com/uploaded.jpg';

      (dotnetInterop.invokeMethodAsync as Mock).mockResolvedValueOnce(expectedUrl);

      const mockFile = new File(['fake image data'], 'photo.jpg', { type: 'image/jpeg' });
      const adapter = mockFileRepository.createUploadAdapter({ file: Promise.resolve(mockFile) });

      const result = await adapter.upload();

      expect(dotnetInterop.invokeMethodAsync).toHaveBeenCalledWith(
        'OnEditorImageUpload',
        expect.objectContaining({
          fileName: 'photo.jpg',
          mimeType: 'image/jpeg',
          payload: expect.any(String),
        }),
      );

      expect(result).toEqual({ default: expectedUrl });
    });

    it('upload adapter should throw when OnEditorImageUpload returns null', async () => {
      const { attachImageUploadAdapter } = createEditorBlazorInterop(element, dotnetInterop);
      const editor = await waitForTestEditor();

      const mockFileRepository: { createUploadAdapter: any; } = { createUploadAdapter: null };

      vi.spyOn(editor.plugins, 'has').mockReturnValue(true);
      vi.spyOn(editor.plugins, 'get').mockReturnValue(mockFileRepository as any);

      await attachImageUploadAdapter();

      (dotnetInterop.invokeMethodAsync as Mock).mockResolvedValueOnce(null);

      const mockFile = new File(['fake image data'], 'photo.jpg', { type: 'image/jpeg' });
      const adapter = mockFileRepository.createUploadAdapter({ file: Promise.resolve(mockFile) });

      await expect(adapter.upload()).rejects.toThrow('OnImageUpload handler returned null');
    });

    it('upload adapter should throw when aborted before file read completes', async () => {
      const { attachImageUploadAdapter } = createEditorBlazorInterop(element, dotnetInterop);
      const editor = await waitForTestEditor();

      const mockFileRepository: { createUploadAdapter: any; } = { createUploadAdapter: null };

      vi.spyOn(editor.plugins, 'has').mockReturnValue(true);
      vi.spyOn(editor.plugins, 'get').mockReturnValue(mockFileRepository as any);

      await attachImageUploadAdapter();

      const mockFile = new File(['fake image data'], 'photo.jpg', { type: 'image/jpeg' });
      const adapter = mockFileRepository.createUploadAdapter({ file: Promise.resolve(mockFile) });

      adapter.abort();

      await expect(adapter.upload()).rejects.toThrow('Upload aborted.');
    });
  });
});
