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
});
