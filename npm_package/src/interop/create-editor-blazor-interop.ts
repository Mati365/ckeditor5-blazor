import type { DotNetInterop } from '../types';
import type { EditorValueSync } from './utils/create-editor-value-sync';

import { ensureEditorElementsRegistered } from '../elements';
import { EditorsRegistry } from '../elements/editor/editors-registry';
import { CKEditor5ChangeDataEvent } from '../elements/editor/plugins/dispatch-editor-roots-change-event';
import { getEditorRootsValues } from '../elements/editor/utils';
import { markElementAsInteractive, shallowEqual } from '../shared';
import { createEditorValueSync, createNoopSync } from './utils/create-editor-value-sync';

/**
 * Creates an interop layer to synchronize a CKEditor 5 instance with a Blazor component.
 *
 * @param element - The root HTML element of the editor component, used to identify the editor instance and attach necessary attributes.
 * @param interop - The .NET object reference to trigger Blazor methods.
 * @returns An object containing lifecycle and synchronization methods.
 */
export function createEditorBlazorInterop(element: HTMLElement, interop: DotNetInterop) {
  const editorId = element.getAttribute('data-cke-editor-id');

  ensureEditorElementsRegistered();
  markElementAsInteractive(element);

  // Placeholder so `setValue` calls queued before the editor is ready are forwarded correctly.
  let sync: EditorValueSync<Record<string, string>> = createNoopSync();
  let afterUnmount: VoidFunction | null = null;

  /**
   * Handles data change events dispatched by the CKEditor plugin.
   * Dispatches updates back to Blazor if the data has changed.
   */
  const onDataChange = (event: Event) => {
    if (!(event instanceof CKEditor5ChangeDataEvent) || event.detail.editorId !== editorId) {
      return;
    }

    if (sync.notifyIfChanged(event.detail.roots)) {
      void interop.invokeMethodAsync('OnChangeEditorData', event.detail.roots);
    }
  };

  document.body.addEventListener(CKEditor5ChangeDataEvent.EVENT_NAME, onDataChange);

  /**
   * Initializes the focus tracker and model listeners for the editor.
   */
  const initializeSynchronization = async () => {
    const editor = await EditorsRegistry.the.waitFor(editorId);
    let editorRef = DotNet.createJSObjectReference(editor);

    // Set up synchronization between the editor and Blazor.
    sync = createEditorValueSync(editor, {
      getCurrentValue: () => getEditorRootsValues(editor),
      applyValue: value => editor.setData(value),
      isEqual: shallowEqual,
    });

    // Notify Blazor of focus changes so it can trigger the appropriate callbacks.
    const onFocusChange = (_evt: unknown, _name: unknown, isFocused: boolean) => {
      const method = isFocused ? 'OnEditorFocus' : 'OnEditorBlur';

      void interop.invokeMethodAsync(method, editorRef);
    };

    editor.ui.focusTracker.on('change:isFocused', onFocusChange);

    // Notify Blazor that the editor instance is ready so the consumer can
    // retain an IJSObjectReference or perform additional JS calls directly.
    // This mirrors the `OnChangeEditorData`/`OnEditorFocus` callbacks that
    // already exist on the .NET side.
    void interop.invokeMethodAsync('OnEditorReady', editorRef);

    // When the Blazor component is disposed, clean up event listeners and dispose the JS object reference.
    afterUnmount = () => {
      editor.ui.focusTracker.off('change:isFocused', onFocusChange);
      DotNet.disposeJSObjectReference(editorRef);
      editorRef = null;
    };
  };

  void initializeSynchronization();

  return {
    /**
     * Updates the editor data from Blazor. If the editor is focused, the update is deferred until blur to avoid interrupting the user.
     */
    setValue: async (value: Record<string, string>) => {
      await EditorsRegistry.the.waitFor(editorId);
      sync.setValue(value);
    },

    /**
     * Cleans up all event listeners when the Blazor component is disposed.
     */
    unmount() {
      document.body.removeEventListener(CKEditor5ChangeDataEvent.EVENT_NAME, onDataChange);
      sync.unmount();
      afterUnmount?.();
    },
  };
}
