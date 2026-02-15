import type { Editor, PluginConstructor } from 'ckeditor5';

import { debounce } from '../../../shared';
import { getEditorRootsValues } from '../utils';

/**
 * Creates a DispatchEditorRootsChangeEvent plugin class.
 */
export async function createDispatchEditorRootsChangeEventPlugin(
  {
    saveDebounceMs,
    editorId,
    targetElement,
  }: {
    saveDebounceMs: number;
    editorId: string;
    targetElement: HTMLElement;
  },
): Promise<PluginConstructor> {
  const { Plugin } = await import('ckeditor5');

  return class DispatchEditorRootsChangeEvent extends Plugin {
    /**
     * The name of the plugin.
     */
    static get pluginName() {
      return 'DispatchEditorRootsChangeEvent' as const;
    }

    /**
     * Initializes the plugin.
     */
    public afterInit(): void {
      const { editor } = this;
      const sync = debounce(saveDebounceMs, this.dispatch);

      editor.model.document.on('change:data', sync);
      editor.once('ready', this.dispatch);
    }

    /**
     * Dispatches a custom event with all roots data.
     */
    private dispatch = (): void => {
      const { editor } = this;

      targetElement.dispatchEvent(
        new CKEditor5ChangeDataEvent({
          editorId,
          editor,
          roots: getEditorRootsValues(editor),
        }),
      );
    };
  };
}

/**
 * A custom event dispatched by the DispatchEditorRootsChangeEvent plugin, containing all editor roots data.
 */
export class CKEditor5ChangeDataEvent extends CustomEvent<CKEditor5ChangeDataEventPayload> {
  static readonly EVENT_NAME = 'ckeditor5:change:data';

  constructor(detail: CKEditor5ChangeDataEventPayload) {
    super(CKEditor5ChangeDataEvent.EVENT_NAME, {
      detail,
      bubbles: true,
    });
  }
}

type CKEditor5ChangeDataEventPayload = {
  editorId: string;
  editor: Editor;
  roots: Record<string, string>;
};
