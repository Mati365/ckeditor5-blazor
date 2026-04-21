import type { DotNetInterop } from '../types';
import type { RootAttributesUpdater } from './utils';
import type { Editor, FileRepository } from 'ckeditor5';

import { ensureEditorElementsRegistered } from '../elements';
import { EditorsRegistry } from '../elements/editor/editors-registry';
import { CKEditor5ChangeDataEvent } from '../elements/editor/plugins/dispatch-editor-roots-change-event';
import { getEditorRootsValues } from '../elements/editor/utils';
import { markElementAsInteractive, shallowEqual } from '../shared';
import { createEditorValueSync, createNoopSync, createRootAttributesUpdater } from './utils';

/**
 * Creates an interop layer to synchronize a CKEditor 5 instance with a Blazor component.
 *
 * @param element - The root HTML element of the editor component, used to identify the editor instance and attach necessary attributes.
 * @param interop - The .NET object reference to trigger Blazor methods.
 * @returns An object containing lifecycle and synchronization methods.
 */
export function createEditorBlazorInterop(element: HTMLElement, interop: DotNetInterop) {
  const editorId = element.getAttribute('data-cke-editor-id');

  let unmounted = false;
  let stopEffect: VoidFunction | null = null;

  let sync = createNoopSync<Record<string, string>>();
  let syncRootAttributes: RootAttributesUpdater | null = null;

  let editorRef: unknown | null = null;

  // Handles data change events dispatched by the CKEditor plugin.
  // Dispatches updates back to Blazor if the data has changed.
  const onDataChange = (event: Event) => {
    if (!(event instanceof CKEditor5ChangeDataEvent) || event.detail.editorId !== editorId) {
      return;
    }

    if (sync.shouldNotify(event.detail.roots)) {
      void interop.invokeMethodAsync('OnChangeEditorData', editorRef!, event.detail.roots);
    }
  };

  stopEffect = EditorsRegistry.the.mountEffect(editorId, (editor) => {
    editorRef = globalThis.DotNet.createJSObjectReference(editor);
    sync = createEditorValueSync(editor, {
      getCurrentValue: () => getEditorRootsValues(editor),
      applyValue: value => editor.setData(value),
      isEqual: shallowEqual,
    });

    syncRootAttributes = createRootAttributesUpdater(editor, 'main');

    // Notify Blazor of focus changes so it can trigger the appropriate callbacks.
    const onFocusChange = (_evt: unknown, _name: unknown, isFocused: boolean) => {
      const method = isFocused ? 'OnEditorFocus' : 'OnEditorBlur';

      void interop.invokeMethodAsync(method, editorRef);
    };

    editor.ui.focusTracker.on('change:isFocused', onFocusChange);

    // Notify Blazor that the editor instance is ready so the consumer can
    // retain an IJSObjectReference or perform additional JS calls directly.
    // This mirrors the `OnChangeEditorData` (which now also drives the public
    // `OnChange` event) as well as `OnEditorFocus` and `OnEditorBlur` callbacks
    // that already exist on the .NET side.
    void interop.invokeMethodAsync('OnEditorReady', editorRef);

    return () => {
      editor.ui.focusTracker.off('change:isFocused', onFocusChange);
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

  ensureEditorElementsRegistered();
  markElementAsInteractive(element);

  return {
    /**
     * Updates the editor data from Blazor. If the editor is focused, the update is deferred until blur to avoid interrupting the user.
     */
    setValue: async (value: Record<string, string>) => {
      if (unmounted) {
        return;
      }

      await EditorsRegistry.the.waitFor(editorId);
      sync.setValue(value);
    },

    /**
     * Updates the root attributes on the editor instance.
     */
    setRootAttributes: async (rootAttributes?: Record<string, unknown> | null) => {
      if (unmounted) {
        return;
      }

      await EditorsRegistry.the.waitFor(editorId);

      syncRootAttributes?.(rootAttributes);
    },

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
     * Installs the custom image upload adapter that delegates uploads to Blazor.
     * This is called lazily from Blazor when the consumer sets the `OnImageUpload` callback
     * to avoid unnecessary overhead for consumers that don't use this feature.
     */
    attachImageUploadAdapter: async () => {
      if (unmounted) {
        return;
      }

      const editor = await EditorsRegistry.the.waitFor(editorId);

      installImageUploadAdapter(editor, interop);
    },
  };
}

/**
 * Installs a custom CKEditor 5 upload adapter that delegates image uploads to Blazor.
 * When the user inserts an image the adapter encodes the file as Base64 and calls
 * `OnEditorImageUpload` on the .NET interop object, which returns the public URL to embed.
 */
function installImageUploadAdapter(editor: Editor, interop: DotNetInterop) {
  if (!editor.plugins.has('FileRepository')) {
    return;
  }

  const fileRepository = editor.plugins.get('FileRepository') as FileRepository;

  fileRepository.createUploadAdapter = (loader: any) => {
    let aborted = false;

    return {
      async upload() {
        const file: File = await loader.file;

        if (aborted) {
          throw new Error('Upload aborted.');
        }

        const payload = await fileToBase64(file);
        const url = await interop.invokeMethodAsync<string | null>('OnEditorImageUpload', {
          fileName: file.name,
          mimeType: file.type,
          payload,
        });

        if (!url) {
          throw new Error(
            'OnImageUpload handler returned null. '
            + 'Make sure the OnImageUpload parameter is set on the <CKE5Editor> component.',
          );
        }

        return { default: url };
      },

      abort() {
        aborted = true;
      },
    };
  };
}

/**
 * Converts a File object to a Base64-encoded string (data-URL prefix stripped).
 */
function fileToBase64(file: File): Promise<string> {
  return new Promise<string>((resolve, reject) => {
    const reader = new FileReader();

    reader.onload = () => {
      const result = reader.result as string;

      /* v8 ignore next -- @preserve */
      const base64 = result.split(',')[1] ?? result;

      resolve(base64);
    };

    /* v8 ignore next -- @preserve */
    reader.onerror = () => reject(reader.error);
    reader.readAsDataURL(file);
  });
}
