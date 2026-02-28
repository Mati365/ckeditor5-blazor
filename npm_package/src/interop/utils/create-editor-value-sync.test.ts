import { ClassicEditor, Essentials, Paragraph } from 'ckeditor5';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import { createEditorValueSync, createNoopSync } from './create-editor-value-sync';

describe('editorValueSync', () => {
  let editor: ClassicEditor;
  let element: HTMLDivElement;

  beforeEach(async () => {
    element = document.createElement('div');
    document.body.appendChild(element);

    editor = await ClassicEditor.create(element, {
      licenseKey: 'GPL',
      plugins: [Essentials, Paragraph],
      toolbar: [],
    });
  });

  afterEach(async () => {
    await editor.destroy();
    element.remove();
  });

  describe('createNoopSync', () => {
    it('noop: shouldNotify always returns false', () => {
      const sync = createNoopSync<string>();

      expect(sync.shouldNotify('hello')).toBe(false);
      expect(sync.shouldNotify('')).toBe(false);
    });

    it('noop: setValue and unmount do not throw', () => {
      const sync = createNoopSync<string>();

      expect(() => sync.setValue('x')).not.toThrow();
      expect(() => sync.unmount()).not.toThrow();
    });
  });

  describe('shouldNotify', () => {
    it('returns true on first call', () => {
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue: v => editor.setData(v),
        isEqual: (a, b) => a === b,
      });

      expect(sync.shouldNotify('first')).toBe(true);
      sync.unmount();
    });

    it('returns false when the same value is passed twice', () => {
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue: v => editor.setData(v),
        isEqual: (a, b) => a === b,
      });

      sync.shouldNotify('same');
      expect(sync.shouldNotify('same')).toBe(false);
      sync.unmount();
    });

    it('returns true when the value changes', () => {
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue: v => editor.setData(v),
        isEqual: (a, b) => a === b,
      });

      sync.shouldNotify('first');
      expect(sync.shouldNotify('second')).toBe(true);
      sync.unmount();
    });
  });

  describe('setValue — editor not focused', () => {
    it('applies value immediately when editor is not focused', () => {
      const applyValue = vi.fn((v: string) => editor.setData(v));
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue,
        isEqual: (a, b) => a === b,
      });

      sync.setValue('<p>hello</p>');

      expect(applyValue).toHaveBeenCalledOnce();
      expect(applyValue).toHaveBeenCalledWith('<p>hello</p>');
      sync.unmount();
    });

    it('skips applying when value matches lastSyncedValue', () => {
      const applyValue = vi.fn((v: string) => editor.setData(v));
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue,
        isEqual: (a, b) => a === b,
      });

      sync.setValue('<p>hello</p>');
      applyValue.mockClear();

      sync.setValue('<p>hello</p>');
      expect(applyValue).not.toHaveBeenCalled();
      sync.unmount();
    });

    it('skips when value equals lastSyncedValue set by shouldNotify', () => {
      const applyValue = vi.fn((v: string) => editor.setData(v));
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue,
        isEqual: (a, b) => a === b,
      });

      sync.shouldNotify('<p>blazor-sent</p>');
      sync.setValue('<p>blazor-sent</p>');

      expect(applyValue).not.toHaveBeenCalled();
      sync.unmount();
    });
  });

  describe('setValue — editor focused', () => {
    it('defers value while editor is focused, applies on blur', async () => {
      const applyValue = vi.fn((v: string) => editor.setData(v));
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue,
        isEqual: (a, b) => a === b,
      });

      editor.editing.view.focus();
      expect(editor.ui.focusTracker.isFocused).toBe(true);

      sync.setValue('<p>deferred</p>');
      expect(applyValue).not.toHaveBeenCalled();

      (document.activeElement as HTMLElement)?.blur();
      await vi.waitFor(() => expect(editor.ui.focusTracker.isFocused).toBe(false));

      expect(applyValue).toHaveBeenCalledOnce();
      expect(applyValue).toHaveBeenCalledWith('<p>deferred</p>');
      sync.unmount();
    });

    it('last value wins when setValue is called multiple times while focused', async () => {
      const applyValue = vi.fn((v: string) => editor.setData(v));
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue,
        isEqual: (a, b) => a === b,
      });

      editor.editing.view.focus();

      sync.setValue('<p>v1</p>');
      sync.setValue('<p>v2</p>');
      sync.setValue('<p>v3</p>');

      (document.activeElement as HTMLElement)?.blur();
      await vi.waitFor(() => expect(editor.ui.focusTracker.isFocused).toBe(false));

      expect(applyValue).toHaveBeenCalledOnce();
      expect(applyValue).toHaveBeenCalledWith('<p>v3</p>');
      sync.unmount();
    });

    it('discards pending value when user types before blur', async () => {
      const applyValue = vi.fn((v: string) => editor.setData(v));
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue,
        isEqual: (a, b) => a === b,
      });

      editor.editing.view.focus();
      sync.setValue('<p>stale</p>');

      // Simulate user typing (internal model change → clears pendingValue)
      editor.model.change((writer) => {
        const root = editor.model.document.getRoot()!;
        const paragraph = writer.createElement('paragraph');
        writer.append(paragraph, root);
        writer.insertText('typed', paragraph);
      });

      (document.activeElement as HTMLElement)?.blur();
      await vi.waitFor(() => expect(editor.ui.focusTracker.isFocused).toBe(false));

      expect(applyValue).not.toHaveBeenCalled();
      sync.unmount();
    });

    it('does not apply pending value on blur when it matches current editor content', async () => {
      const applyValue = vi.fn((v: string) => editor.setData(v));

      editor.setData('<p>already here</p>');

      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue,
        isEqual: (a, b) => a === b,
      });

      editor.editing.view.focus();
      sync.setValue('<p>already here</p>');

      (document.activeElement as HTMLElement)?.blur();
      await vi.waitFor(() => expect(editor.ui.focusTracker.isFocused).toBe(false));

      expect(applyValue).not.toHaveBeenCalled();
      sync.unmount();
    });
  });

  describe('unmount', () => {
    it('stops applying deferred values after unmount', async () => {
      const applyValue = vi.fn((v: string) => editor.setData(v));
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue,
        isEqual: (a, b) => a === b,
      });

      editor.editing.view.focus();
      sync.setValue('<p>pending</p>');

      sync.unmount();

      (document.activeElement as HTMLElement)?.blur();
      await vi.waitFor(() => expect(editor.ui.focusTracker.isFocused).toBe(false));

      expect(applyValue).not.toHaveBeenCalled();
    });

    it('is safe to call multiple times', () => {
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue: v => editor.setData(v),
        isEqual: (a, b) => a === b,
      });

      expect(() => {
        sync.unmount();
        sync.unmount();
      }).not.toThrow();
    });
  });

  describe('integration', () => {
    it('stale Blazor update does not overwrite user edits', async () => {
      const applyValue = vi.fn((v: string) => editor.setData(v));
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue,
        isEqual: (a, b) => a === b,
      });

      sync.shouldNotify('<p>Hello</p>');

      editor.editing.view.focus();

      // Blazor sends back the old (stale) value while user is typing
      sync.setValue('<p>Hello</p>');

      // User types something — change:data fires and discards the pending value
      editor.model.change((writer) => {
        const root = editor.model.document.getRoot()!;
        const paragraph = writer.createElement('paragraph');
        writer.append(paragraph, root);
        writer.insertText(' World', paragraph);
      });

      (document.activeElement as HTMLElement)?.blur();
      await vi.waitFor(() => expect(editor.ui.focusTracker.isFocused).toBe(false));

      expect(applyValue).not.toHaveBeenCalled();
      sync.unmount();
    });

    it('fresh Blazor value is applied when editor is idle', () => {
      const applyValue = vi.fn((v: string) => editor.setData(v));
      const sync = createEditorValueSync(editor, {
        getCurrentValue: () => editor.getData(),
        applyValue,
        isEqual: (a, b) => a === b,
      });

      sync.shouldNotify('<p>old</p>');
      applyValue.mockClear();

      sync.setValue('<p>brand new</p>');

      expect(applyValue).toHaveBeenCalledWith('<p>brand new</p>');
      sync.unmount();
    });
  });
});
