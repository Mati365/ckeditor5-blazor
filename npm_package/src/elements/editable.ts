import type { WaitForInteractiveResult } from '../shared';
import type { MultiRootEditor } from 'ckeditor5';

import { CKEditor5BlazorError } from '../ckeditor5-blazor-error';
import { debounce, isEmptyObject, waitForDOMReady, waitForInteractiveAttribute } from '../shared';
import { EditorsRegistry } from './editor/editors-registry';
import { queryAllEditorIds } from './editor/utils';

/**
 * Editable hook for Blazor. It allows you to create editables for multi-root editors.
 */
export class EditableComponentElement extends HTMLElement {
  /**
   * Stops observing the editor registry and immediately runs any pending cleanup.
   */
  private unmountEffect: VoidFunction | null = null;

  /**
   * Wait result for the interactive attribute.
   */
  private interactiveWait?: WaitForInteractiveResult;

  /**
   * Mounts the editable component.
   */
  async connectedCallback() {
    await waitForDOMReady();

    this.interactiveWait = waitForInteractiveAttribute(this);
    await this.interactiveWait.promise;
    await this.initializeEditable();
  }

  /**
   * Initializes the editable instance.
   */
  private async initializeEditable(): Promise<void> {
    if (!this.hasAttribute('data-cke-editor-id')) {
      this.setAttribute('data-cke-editor-id', queryAllEditorIds()[0]!);
    }

    const editorId = this.getAttribute('data-cke-editor-id');
    const rootName = this.getAttribute('data-cke-root-name');
    const rootAttributes = JSON.parse(this.getAttribute('data-cke-root-attributes') || '{}');
    const content = this.getAttribute('data-cke-content');
    const saveDebounceMs = Number.parseInt(this.getAttribute('data-cke-save-debounce-ms')!, 10);

    /* v8 ignore next if -- @preserve */
    if (!editorId || !rootName) {
      throw new CKEditor5BlazorError('Editor ID or Root Name is missing.');
    }

    this.style.display = 'block';

    this.unmountEffect = EditorsRegistry.the.mountEffect(editorId, (editor: MultiRootEditor) => {
      if (!this.isConnected) {
        return;
      }

      const { ui, editing, model } = editor;

      const input = this.querySelector('input') as HTMLInputElement | null;
      const root = model.document.getRoot(rootName);

      if (root?.isAttached()) {
        // If the newly added root already exists, but the newly added editable has content,
        // we need to update the root data with the editable content.
        if (content !== null) {
          const data = editor.getData({ rootName });

          if (data && data !== content) {
            editor.setData({
              [rootName]: content,
            });
          }
        }

        // Assign attributes to the root if they are not empty.
        // This allows users to add custom attributes to the root element of the editable.
        if (!isEmptyObject(rootAttributes)) {
          editor.model.change((writer) => {
            writer.setAttributes(rootAttributes, root);
          });
        }

        return;
      }

      editor.addRoot(rootName, {
        isUndoable: false,
        attributes: { ...rootAttributes },
        ...content !== null && {
          data: content,
        },
      });

      const contentElement = this.querySelector('[data-cke-editable-content]') as HTMLElement | null;
      const editable = ui.view.createEditable(rootName, contentElement!);

      ui.addEditable(editable);
      editing.view.forceRender();

      // Sync data with socket and input element.
      const sync = () => {
        if (!model.document.getRoot(rootName)?.isAttached()) {
          return;
        }

        const html = editor.getData({ rootName });

        if (input) {
          input.value = html;
          input.dispatchEvent(new Event('input'));
        }

        this.dispatchEvent(new CustomEvent('change', {
          detail: {
            value: html,
          },
        }));
      };

      const debouncedSync = debounce(saveDebounceMs, sync);

      editor.model.document.on('change:data', debouncedSync);
      sync();

      return () => {
        editor.model.document.off('change:data', debouncedSync);

        /* v8 ignore else -- @preserve */
        if (editor.state !== 'destroyed' && rootName) {
          const root = editor.model.document.getRoot(rootName);

          /* v8 ignore else -- @preserve */
          if (root && 'detachEditable' in editor) {
            // Detaching editables seem to be buggy when something removed DOM element of the editable (e.g. Blazor re-render) before
            // the editable is unmounted. To prevent errors in such cases, we will try to detach the editable if it exists, but ignore errors.
            try {
              /* v8 ignore else -- @preserve */
              if (editor.ui.view.editables[rootName]) {
                editor.detachEditable(root);
              }
            }
            catch (err) {
              // Ignore errors when detaching editable.
              /* v8 ignore next -- @preserve */
              console.error('Unable unmount editable from root:', err);
            }

            if (root.isAttached()) {
              editor.detachRoot(rootName, false);
            }
          }
        }
      };
    });
  }

  /**
   * Destroys the editable component. Unmounts root from the editor.
   */
  disconnectedCallback() {
    // Disconnect the observer if present.
    this.interactiveWait?.disconnect();

    // Let's hide the element during destruction to prevent flickering.
    this.style.display = 'none';

    // Stop observing the registry and run cleanup immediately.
    this.unmountEffect?.();
    this.unmountEffect = null;
  }
}
