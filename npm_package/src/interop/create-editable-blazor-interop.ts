import type { DotNetInterop } from '../types';
import type { RootAttributesUpdater } from './utils';

import { EditorsRegistry } from '../elements/editor/editors-registry';
import { CKEditor5ChangeDataEvent } from '../elements/editor/plugins/dispatch-editor-roots-change-event';
import { queryAllEditorIds } from '../elements/editor/utils';
import { markElementAsInteractive } from '../shared';
import { createEditorValueSync, createNoopSync, createRootAttributesUpdater } from './utils';

/**
 * Creates an interop layer to synchronize a single CKEditor 5 editable root with a Blazor component.
 *
 * @param element - The root HTML element of the editable component, used to identify
 * the editable instance and attach necessary attributes.
 * @param interop - The .NET object reference to trigger Blazor methods.
 * @returns An object containing lifecycle and synchronization methods.
 */
export function createEditableBlazorInterop(element: HTMLElement, interop: DotNetInterop) {
  const editorId = element.getAttribute('data-cke-editor-id') ?? queryAllEditorIds()[0]!;
  const rootName = element.getAttribute('data-cke-root-name') ?? 'main';

  let unmounted = false;
  let stopEffect: VoidFunction | null = null;

  let editorRef: unknown | null = null;

  let sync = createNoopSync<string>();
  let syncRootAttributes: RootAttributesUpdater | null = null;

  /**
   * Handles data change events dispatched by the CKEditor plugin.
   * Filters by both editorId and rootName, then notifies Blazor if the root value changed.
   * The callback now includes a JS object reference to the underlying editor instance.
   */
  const onDataChange = (event: Event) => {
    if (!(event instanceof CKEditor5ChangeDataEvent) || event.detail.editorId !== editorId) {
      return;
    }

    const newValue = event.detail.roots[rootName];

    if (newValue === undefined) {
      return;
    }

    if (sync.shouldNotify(newValue)) {
      void interop.invokeMethodAsync('OnChangeEditableData', editorRef, newValue);
    }
  };

  stopEffect = EditorsRegistry.the.mountEffect(editorId, (editor) => {
    editorRef = globalThis.DotNet.createJSObjectReference(editor);

    sync = createEditorValueSync(editor, {
      getCurrentValue: () => editor.getData({ rootName }) ?? '',
      applyValue: value => editor.setData({ [rootName]: value }),
      isEqual: (a, b) => a === b,
    });

    syncRootAttributes = createRootAttributesUpdater(editor, rootName);

    return () => {
      sync.unmount();

      /* v8 ignore else -- @preserve */
      if (editorRef) {
        globalThis.DotNet?.disposeJSObjectReference(editorRef);
        editorRef = null;
      }

      syncRootAttributes = null;
    };
  });

  document.body.addEventListener(CKEditor5ChangeDataEvent.EVENT_NAME, onDataChange);
  markElementAsInteractive(element);

  return {
    /**
     * Cleans up all event listeners when the Blazor component is disposed.
     */
    unmount() {
      if (unmounted) {
        return;
      }

      document.body.removeEventListener(CKEditor5ChangeDataEvent.EVENT_NAME, onDataChange);

      stopEffect?.();
      stopEffect = null;

      unmounted = true;
    },

    /**
     * Updates this editable root's data from Blazor.
     * If the editor is focused, the update is deferred until blur to avoid interrupting the user.
     */
    setValue: async (value: string) => {
      if (unmounted) {
        return;
      }

      await EditorsRegistry.the.waitFor(editorId);
      sync.setValue(value);
    },

    /**
     * Updates the root attributes on the editor. This is useful when the Blazor component
     * re-renders with new root attributes.
     */
    setRootAttributes: async (rootAttributes?: Record<string, unknown> | null) => {
      if (unmounted) {
        return;
      }

      await EditorsRegistry.the.waitFor(editorId);

      syncRootAttributes?.(rootAttributes);
    },
  };
}
