import type { DotNetInterop } from '../types';
import type { Mock } from 'vitest';

import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import {
  createDotnet,
  createDotNetInteropMock,
  DEFAULT_TEST_EDITOR_ID,
  renderTestEditable,
  renderTestEditor,
  waitForDestroyAllEditors,
  waitForTestEditor,
} from '~/test-utils';

import { CKEditor5ChangeDataEvent } from '../elements/editor/plugins/dispatch-editor-roots-change-event';
import { ensureEditorElementsRegistered } from '../elements/ensure-editor-elements-registered';
import { createEditableBlazorInterop } from './create-editable-blazor-interop';

describe('createEditableBlazorInterop', () => {
  let element: HTMLElement;
  let dotnetInterop: DotNetInterop;

  beforeEach(async () => {
    document.body.innerHTML = '';
    ensureEditorElementsRegistered();

    dotnetInterop = createDotNetInteropMock();
    globalThis.DotNet = createDotnet();

    element = renderTestEditable({}, {
      interactive: false,
    });

    renderTestEditor();
    await waitForTestEditor();
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

    createEditableBlazorInterop(element, dotnetInterop);

    const editor = await waitForTestEditor();

    expect(editor).toBeDefined();
    expect(element.getAttribute('data-cke-interactive')).toBe('true');
  });

  it('defaults rootName to "main" when attribute is missing', async () => {
    element.removeAttribute('data-cke-root-name');

    createEditableBlazorInterop(element, dotnetInterop);

    const editor = await waitForTestEditor();
    const changeEvent = new CKEditor5ChangeDataEvent({
      editorId: DEFAULT_TEST_EDITOR_ID,
      editor,
      roots: { main: 'changed' },
    });

    document.body.dispatchEvent(changeEvent);

    expect(dotnetInterop.invokeMethodAsync).toHaveBeenCalledWith(
      'OnChangeEditableData',
      expect.anything(),
      'changed',
    );
  });

  it('falls back to the first available editor id when data-cke-editor-id attribute is missing', async () => {
    element.removeAttribute('data-cke-editor-id');

    createEditableBlazorInterop(element, dotnetInterop);

    const editor = await waitForTestEditor();
    const changeEvent = new CKEditor5ChangeDataEvent({
      editorId: DEFAULT_TEST_EDITOR_ID,
      editor,
      roots: { main: 'fallback changed' },
    });

    document.body.dispatchEvent(changeEvent);

    expect(dotnetInterop.invokeMethodAsync).toHaveBeenCalledWith(
      'OnChangeEditableData',
      expect.anything(),
      'fallback changed',
    );
  });

  it('should ignore ckeditor5 change events from other editors', async () => {
    createEditableBlazorInterop(element, dotnetInterop);

    const editor = await waitForTestEditor();

    const changeEvent = new CKEditor5ChangeDataEvent({
      editorId: 'other-editor',
      editor,
      roots: { main: 'changed' },
    });

    document.body.dispatchEvent(changeEvent);

    expect(dotnetInterop.invokeMethodAsync).not.toHaveBeenCalledWith(
      'OnChangeEditableData',
      expect.anything(),
      expect.anything(),
    );
  });

  it('should ignore events for an unknown root name', async () => {
    const custom = renderTestEditable({ rootName: 'other' }, { interactive: false });

    const interop = createEditableBlazorInterop(custom, dotnetInterop);
    const editor = await waitForTestEditor();

    const changeEvent = new CKEditor5ChangeDataEvent({
      editorId: DEFAULT_TEST_EDITOR_ID,
      editor,
      roots: { unknown: 'value' },
    });

    (dotnetInterop.invokeMethodAsync as Mock).mockClear();
    document.body.dispatchEvent(changeEvent);
    expect(dotnetInterop.invokeMethodAsync).not.toHaveBeenCalled();

    interop.unmount();
  });

  describe('setValue', () => {
    it('should be possible to call `setValue` while editor is initializing', async () => {
      const { setValue } = createEditableBlazorInterop(element, dotnetInterop);

      expect(() => setValue('test')).not.toThrow();

      const editor = await waitForTestEditor();

      expect(editor.getData()).toBe('<p>test</p>');
    });

    it('should not set data if the interop is unmounted before the editor is ready', async () => {
      const { setValue, unmount } = createEditableBlazorInterop(element, dotnetInterop);

      unmount();

      expect(() => setValue('test')).not.toThrow();

      const editor = await waitForTestEditor();

      expect(editor.getData()).toBe('<p>Initial content</p>');
    });

    it('should delay setting data if the editor is focused', async () => {
      const { setValue } = createEditableBlazorInterop(element, dotnetInterop);
      const editor = await waitForTestEditor();

      editor.ui.focusTracker.isFocused = true;

      await setValue('focused update');

      expect(editor.getData()).toBe('<p>Initial content</p>');

      editor.ui.focusTracker.isFocused = false;

      await vi.waitFor(() => expect(editor.getData()).toBe('<p>focused update</p>'));
    });

    it('treats null/undefined editor data as empty string when syncing after focus', async () => {
      const { setValue } = createEditableBlazorInterop(element, dotnetInterop);

      const editor = await waitForTestEditor();
      const originalGetData = editor.getData;

      (editor as any).getData = () => null;

      editor.ui.focusTracker.isFocused = true;
      await setValue('from-blazor');
      expect(editor.getData()).toBe(null);

      editor.ui.focusTracker.isFocused = false;
      (editor as any).getData = originalGetData;
      expect(editor.getData()).toBe('<p>from-blazor</p>');
    });
  });

  describe('unmount', () => {
    it('should remove event listeners and prevent future updates', async () => {
      const interop = createEditableBlazorInterop(element, dotnetInterop);
      const editor = await waitForTestEditor();

      const changeEvent = new CKEditor5ChangeDataEvent({
        editorId: DEFAULT_TEST_EDITOR_ID,
        editor,
        roots: { main: 'changed' },
      });

      document.body.dispatchEvent(changeEvent);

      expect(dotnetInterop.invokeMethodAsync).toHaveBeenCalledWith(
        'OnChangeEditableData',
        expect.anything(),
        'changed',
      );

      (dotnetInterop.invokeMethodAsync as Mock).mockClear();

      interop.unmount();
      document.body.dispatchEvent(changeEvent);

      expect(dotnetInterop.invokeMethodAsync).not.toBeCalled();
    });

    it('should not crash if called twice', async () => {
      const interop = createEditableBlazorInterop(element, dotnetInterop);
      await waitForTestEditor();

      expect(() => {
        interop.unmount();
        interop.unmount();
      }).not.toThrow();
    });
  });
});
