import type { MultiRootEditor } from 'ckeditor5';

import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import {
  createEditorPreset,
  renderTestEditable,
  renderTestEditor,
  waitForDestroyAllEditors,
  waitForTestEditor,
} from '~/test-utils';

import { timeout } from '../shared';
import { ensureEditorElementsRegistered } from './ensure-editor-elements-registered';

describe('editable component', () => {
  beforeEach(() => {
    document.body.innerHTML = '';
    ensureEditorElementsRegistered();
  });

  afterEach(async () => {
    vi.useRealTimers();
    vi.resetAllMocks();

    await waitForDestroyAllEditors();

    document.body.innerHTML = '';
  });

  describe('mounting editable', () => {
    it('should add editable root to the editor after mounting editor (empty editor)', async () => {
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {},
      });

      const editor = await waitForTestEditor<MultiRootEditor>();

      renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo component</p>',
      });

      await vi.waitFor(() => {
        expect(editor.getData({ rootName: 'foo' })).toBe('<p>Initial foo component</p>');
      });
    });

    it('should add editable root to the editor after mounting editor (non-empty editor, other editable defined before)', async () => {
      renderTestEditable({
        rootName: 'bar',
        content: '<p>Initial bar content</p>',
      });

      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {
          bar: '<p>Initial bar content</p>',
        },
      });

      const editor = await waitForTestEditor<MultiRootEditor>();

      renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo content</p>',
      });

      await vi.waitFor(() => {
        expect(editor.getData({ rootName: 'foo' })).toBe('<p>Initial foo content</p>');
        expect(editor.getData({ rootName: 'bar' })).toBe('<p>Initial bar content</p>');
      });
    });

    it('should add editable root to the editor after mounting editor (non-empty editor, other editable defined after)', async () => {
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {
          bar: '<p>Initial bar content</p>',
        },
      });

      renderTestEditable({
        rootName: 'bar',
        content: '<p>Initial bar content</p>',
      });

      const editor = await waitForTestEditor<MultiRootEditor>();

      renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo content</p>',
      });

      await vi.waitFor(() => {
        expect(editor.getData({ rootName: 'foo' })).toBe('<p>Initial foo content</p>');
        expect(editor.getData({ rootName: 'bar' })).toBe('<p>Initial bar content</p>');
      });
    });

    it('should do nothing if adding existing root (without provided content)', async () => {
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {
          main: '<p>Initial main content</p>',
        },
      });

      renderTestEditable({
        rootName: 'main',
        content: null,
      });

      const editor = await waitForTestEditor<MultiRootEditor>();

      await vi.waitFor(() => {
        expect(editor.getData({ rootName: 'main' })).toBe('<p>Initial main content</p>');
      });
    });

    it('should update existing root content if added existing root with provided content', async () => {
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {
          main: '<p>Initial main content</p>',
        },
      });

      renderTestEditable({
        rootName: 'main',
        content: '<p>Updated main content</p>',
      });

      const editor = await waitForTestEditor<MultiRootEditor>();

      await vi.waitFor(() => {
        expect(editor.getData({ rootName: 'main' })).toBe('<p>Updated main content</p>');
      });
    });

    it('should apply provided root attributes to the editable root', async () => {
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {},
      });

      const editor = await waitForTestEditor<MultiRootEditor>();

      renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo component</p>',
        rootAttributes: {
          'data-test-attr': 'foo-root',
        },
      });

      await vi.waitFor(() => {
        expect(editor.model.document.getRoot('foo')!.getAttribute('data-test-attr')).toBe('foo-root');
      });
    });

    it('should apply provided root attributes when editable is mounted after root already exists', async () => {
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {},
      });

      const editor = await waitForTestEditor<MultiRootEditor>();

      renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo component</p>',
        rootAttributes: {
          'data-test-attr': 'foo-root',
        },
      });

      editor.model.change((writer) => {
        writer.addRoot('foo-2');
      });

      renderTestEditable({
        rootName: 'foo-2',
        content: '<p>Initial foo-2 component</p>',
        rootAttributes: {
          'data-test-attr': 'foo-2-root',
        },
      });

      await vi.waitFor(() => {
        const { document } = editor.model;

        expect(document.getRoot('foo')!.getAttribute('data-test-attr')).toBe('foo-root');
        expect(document.getRoot('foo-2')!.getAttribute('data-test-attr')).toBe('foo-2-root');
      });
    });

    it('should auto-assign editor ID if not provided', async () => {
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {},
      });

      await waitForTestEditor<MultiRootEditor>();

      const editable = renderTestEditable({
        editorId: '',
        rootName: 'foo',
        content: '<p>Initial foo component</p>',
      });

      await vi.waitFor(() => {
        expect(editable.getAttribute('data-cke-editor-id')).toBe('test-editor');
      });
    });

    it('should not initialize editable if element is disconnected before editor is ready', async () => {
      const el = renderTestEditable({
        rootName: 'foo',
        content: '<p>Foo</p>',
      });
      el.remove();

      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {},
      });

      const editor = await waitForTestEditor<MultiRootEditor>();

      await timeout(10);

      expect(editor.model.document.getRoot('foo')).toBe(null);
    });
  });

  describe('input value synchronization', () => {
    let editor: MultiRootEditor;

    beforeEach(async () => {
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {},
      });

      editor = await waitForTestEditor<MultiRootEditor>();
      vi.useFakeTimers();
    });

    it('should not crash if input is not present', async () => {
      renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo component</p>',
      }, { withInput: false });

      await vi.waitFor(() => {
        expect(editor.getData({ rootName: 'foo' })).toBe('<p>Initial foo component</p>');
      });
    });

    it('should synchronize input value after mounting editable', async () => {
      const element = renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo component</p>',
      }, { withInput: true });

      const input = element.querySelector('input')!;

      await vi.waitFor(() => {
        expect(input.value).toBe('<p>Initial foo component</p>');
      });
    });

    it('should debounce input value synchronization', async () => {
      const element = renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo component</p>',
        saveDebounceMs: 500,
      }, { withInput: true });

      const input = element.querySelector('input')!;

      await vi.waitFor(() => {
        expect(input.value).toBe('<p>Initial foo component</p>');
      });

      editor.setData({
        foo: '<p>Modified foo content</p>',
      });

      vi.advanceTimersByTime(300);
      expect(input.value).toBe('<p>Initial foo component</p>');

      vi.advanceTimersByTime(300);
      expect(input.value).toBe('<p>Modified foo content</p>');
    });
  });

  describe('web component events', () => {
    let editor: MultiRootEditor;

    beforeEach(async () => {
      vi.useFakeTimers();
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {},
      });
      editor = await waitForTestEditor<MultiRootEditor>();
    });

    it('should emit change event when editor data changes', async () => {
      const element = renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial content</p>',
        saveDebounceMs: 100,
      });

      const changeSpy = vi.fn();
      element.addEventListener('change', changeSpy);

      await vi.waitFor(() => {
        expect(editor.getData({ rootName: 'foo' })).toBe('<p>Initial content</p>');
      });

      // Clear spy after initial sync
      changeSpy.mockClear();

      editor.setData({ foo: '<p>New content</p>' });

      vi.advanceTimersByTime(150);

      expect(changeSpy).toHaveBeenCalledTimes(1);
      expect((changeSpy.mock.lastCall![0] as CustomEvent).detail.value).toBe('<p>New content</p>');
    });
  });

  describe('destroy', () => {
    it('should detach editable root from editor on component unmount', async () => {
      const consoleSpy = vi.spyOn(console, 'warn').mockImplementation(() => undefined);

      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {},
      });

      const editor = await waitForTestEditor<MultiRootEditor>();
      const element = renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo content</p>',
      });

      await vi.waitFor(() => {
        expect(editor.model.document.getRoot('foo')!.isAttached()).toBe(true);
      });

      element.remove();

      await vi.waitFor(() => {
        expect(editor.model.document.getRoot('foo')?.isAttached()).toBe(false);
      });

      consoleSpy.mockRestore();
    });

    it('should hide element during destruction', async () => {
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {},
      });

      const editor = await waitForTestEditor<MultiRootEditor>();
      const element = renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo content</p>',
      });

      await vi.waitFor(() => {
        expect(editor.getData({ rootName: 'foo' })).toBeDefined();
      });

      element.remove();

      expect(element.style.display).toBe('none');
    });

    it('should not crash if editor was destroyed before editable', async () => {
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {},
      });

      const editorElement = document.querySelector('cke5-editor')!;
      const editor = await waitForTestEditor<MultiRootEditor>();

      const element = renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo content</p>',
      });

      await vi.waitFor(() => {
        expect(editor.getData({ rootName: 'foo' })).toBeDefined();
      });

      editorElement.remove();

      await vi.waitFor(() => expect(editor.state).toBe('destroyed'));

      expect(() => element.remove()).not.toThrow();
    });

    it('should not attempt to detach root again if it is already detached', async () => {
      renderTestEditor({
        preset: createEditorPreset('multiroot'),
        content: {},
      });

      const editor = await waitForTestEditor<MultiRootEditor>();
      const element = renderTestEditable({
        rootName: 'foo',
        content: '<p>Initial foo content</p>',
      });

      await vi.waitFor(() => {
        expect(editor.model.document.getRoot('foo')!.isAttached()).toBe(true);
      });

      const detachSpy = vi.spyOn(editor, 'detachRoot');
      editor.detachRoot('foo', false);
      expect(detachSpy).toHaveBeenCalledTimes(1);

      detachSpy.mockClear();

      element.remove();
      await timeout(10);

      expect(detachSpy).not.toHaveBeenCalled();
    });
  });
});
