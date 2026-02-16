import { ensureEditorElementsRegistered } from '../elements';
import { EditorsRegistry } from '../elements/editor/editors-registry';
import { CKEditor5ChangeDataEvent } from '../elements/editor/plugins/dispatch-editor-roots-change-event';
import { getEditorRootsValues } from '../elements/editor/utils';
import { shallowEqual } from '../shared';

/**
 * Creates an interop layer to synchronize a CKEditor 5 instance with a Blazor component.
 * @param editorId - The unique identifier of the CKEditor instance.
 * @param interop - The .NET object reference to trigger Blazor methods.
 * @returns An object containing lifecycle and synchronization methods.
 */
export function createEditorBlazorInterop(editorId: string, interop: DotNetInterop) {
  const element = document.querySelector(`cke5-editor[data-cke-editor-id="${editorId}"]`) as HTMLElement | null;

  /* v8 ignore next if -- @preserve */
  if (!element) {
    throw new Error(`Editor element with ID "${editorId}" not found.`);
  }

  ensureEditorElementsRegistered();
  element?.setAttribute('data-cke-interactive', '');

  /**
   * Internal state to track values and prevent unnecessary updates or race conditions.
   */
  const state = {
    /** Value received from Blazor while the editor was focused, pending application on blur. */
    pendingValue: null as Record<string, string> | null,

    /** The last value sent to or received from Blazor to prevent circular updates. */
    lastSyncedValue: null as Record<string, string> | null,
  };

  /**
   * Handles data change events dispatched by the CKEditor plugin.
   * Dispatches updates back to Blazor if the data has changed.
   */
  const onDataChange = (event: Event) => {
    if (!(event instanceof CKEditor5ChangeDataEvent) || event.detail.editorId !== editorId) {
      return;
    }

    const newRoots = event.detail.roots;

    // Any manual user change invalidates a pending value from the server
    state.pendingValue = null;

    if (!state.lastSyncedValue || !shallowEqual(state.lastSyncedValue, newRoots)) {
      state.lastSyncedValue = newRoots;
      void interop.invokeMethodAsync('OnChangeEditorData', newRoots);
    }
  };

  document.body.addEventListener(CKEditor5ChangeDataEvent.EVENT_NAME, onDataChange);

  /**
   * Initializes the focus tracker and model listeners for the editor.
   */
  const initializeSynchronization = async () => {
    const editor = await EditorsRegistry.the.waitFor(editorId);

    // If the model changes internally, the pending external value is no longer valid
    editor.model.document.on('change:data', () => {
      state.pendingValue = null;
    });

    // Handle focus changes to apply pending values safely
    editor.ui.focusTracker.on('change:isFocused', (_, __, isFocused) => {
      if (isFocused || !state.pendingValue) {
        return;
      }

      const currentData = getEditorRootsValues(editor);
      if (!shallowEqual(currentData, state.pendingValue)) {
        editor.setData(state.pendingValue);
      }

      state.pendingValue = null;
    });
  };

  void initializeSynchronization();

  return {
    /**
     * Cleans up event listeners when the Blazor component is disposed.
     */
    unmount() {
      document.body.removeEventListener(CKEditor5ChangeDataEvent.EVENT_NAME, onDataChange);
    },

    /**
     * Updates the editor data from Blazor. If the editor is focused, the update is deferred until blur to avoid interrupting the user.
     */
    setValue: async (value: Record<string, string>) => {
      const editor = await EditorsRegistry.the.waitFor(editorId);

      // Defer update if user is currently typing
      if (editor.ui.focusTracker.isFocused) {
        state.pendingValue = value;
        return;
      }

      // Only set data if it differs from the last known state
      if (!state.lastSyncedValue || !shallowEqual(state.lastSyncedValue, value)) {
        state.lastSyncedValue = value;
        editor.setData(value);
      }
    },
  };
}

/**
 * Represents the .NET interop helper for communication with Blazor.
 */
type DotNetInterop = {
  invokeMethodAsync: (methodName: string, ...args: any[]) => Promise<void>;
};
