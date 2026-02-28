import type { Editor } from 'ckeditor5';

/**
 * Creates a focus-aware value synchronization layer for a CKEditor 5 instance.
 *
 * Handles the shared mechanics present in both the Editor and Editable Blazor interops:
 * - Tracking `lastSyncedValue` to prevent circular Blazor ↔ JS updates.
 * - Storing a `pendingValue` when a Blazor update arrives while the editor is focused,
 *   and applying it safely once the user blurs.
 * - Registering and cleaning up the `change:data` and `change:isFocused` editor listeners.
 *
 * @param editor - The CKEditor 5 editor instance to synchronize with.
 * @param options - Callbacks and equality check specific to the consumer (editor vs editable).
 * @param options.getCurrentValue - Returns the current value of the tracked root(s) from the editor.
 * @param options.applyValue - Applies the given value to the editor (e.g. via `editor.setData`).
 * @param options.isEqual - Returns true if two values are considered equal, used to avoid redundant updates.
 * @returns An object with `setValue`, `shouldNotify`, and `unmount`.
 */
export function createEditorValueSync<T>(
  editor: Editor,
  options: {

    /**
     * Returns the current value of the tracked root(s) from the editor.
     * Called to compare against the pending value before applying it.
     */
    getCurrentValue: () => T;

    /**
     * Applies the given value to the editor (e.g. via `editor.setData`).
     */
    applyValue: (value: T) => void;

    /**
     * Returns true if two values are considered equal, used to avoid redundant
     * updates in both directions.
     */
    isEqual: (a: T, b: T) => boolean;
  },
): EditorValueSync<T> {
  const state = {
    /** Value received from Blazor while the editor was focused, pending application on blur. */
    pendingValue: null as T | null,

    /** The last value sent to or received from Blazor to prevent circular updates. */
    lastSyncedValue: null as T | null,
  };

  /**
   * Any internal model change means the pending external value is stale — discard it.
   */
  const onChangeData = () => {
    state.pendingValue = null;
  };

  /**
   * When the editor loses focus, apply any value that Blazor sent while the user was typing.
   */
  const onFocusChange = (_evt: unknown, _name: unknown, isFocused: boolean) => {
    if (isFocused || state.pendingValue === null) {
      return;
    }

    const current = options.getCurrentValue();

    if (!options.isEqual(current, state.pendingValue)) {
      options.applyValue(state.pendingValue);
    }

    state.pendingValue = null;
  };

  editor.model.document.on('change:data', onChangeData);
  editor.ui.focusTracker.on('change:isFocused', onFocusChange);

  return {
    /**
     * Removes all editor listeners registered by this sync instance.
     * Call this when the Blazor component is disposed.
     */
    unmount() {
      editor.model.document.off('change:data', onChangeData);
      editor.ui.focusTracker.off('change:isFocused', onFocusChange);
    },

    /**
     * Checks whether the given value differs from the last synced value and, if so,
     * updates `lastSyncedValue` and returns `true` to signal that Blazor should be notified.
     *
     * Call this from the `CKEditor5ChangeDataEvent` handler to conditionally invoke
     * the .NET interop method.
     */
    shouldNotify(value: T): boolean {
      if (state.lastSyncedValue !== null && options.isEqual(state.lastSyncedValue, value)) {
        return false;
      }

      state.lastSyncedValue = value;
      return true;
    },

    /**
     * Pushes a new value from Blazor into the editor.
     * If the editor is currently focused, the update is deferred until blur.
     * If the value matches the last synced state, the update is skipped entirely.
     */
    setValue(value: T) {
      if (editor.ui.focusTracker.isFocused) {
        state.pendingValue = value;
        return;
      }

      if (state.lastSyncedValue !== null && options.isEqual(state.lastSyncedValue, value)) {
        return;
      }

      state.lastSyncedValue = value;
      options.applyValue(value);
    },
  };
}

/**
 * Returns a no-op {@link EditorValueSync} used as a placeholder before the real editor
 * instance is available, so callers never have to null-check `sync`.
 */
export function createNoopSync<T>(): EditorValueSync<T> {
  return {
    unmount() {},
    shouldNotify(_value: T): boolean { return false; },
    setValue(_value: T) {},
  };
}

/**
 * The public interface returned by {@link createEditorValueSync} and {@link createNoopSync}.
 * Typed over the value shape `T` so consumers can declare `sync` variables without
 * repeating the full return type.
 */
export type EditorValueSync<T> = {
  /** Removes all editor listeners registered by this sync instance. */
  unmount: () => void;

  /**
   * Checks whether the given value differs from the last synced value and, if so,
   * updates `lastSyncedValue` and returns `true` to signal that Blazor should be notified.
   */
  /**
   * Called by the consumer to determine if a value change should trigger a
   * notification. Returns `true` when the value differs from the last one that
   * was synced and updates internal tracking.
   */
  shouldNotify: (value: T) => boolean;

  /**
   * Pushes a new value from Blazor into the editor.
   * Deferred until blur when the editor is focused; skipped when value is unchanged.
   */
  setValue: (value: T) => void;
};
