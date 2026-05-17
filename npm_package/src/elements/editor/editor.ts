import type { WaitForInteractiveResult } from '../../shared';
import type { EditorId, EditorLanguage, EditorPreset } from './typings';

import {
  isEmptyObject,
  waitFor,
  waitForDOMReady,
  waitForInteractiveAttribute,
} from '../../shared';
import { ContextsRegistry } from '../context';
import { EditorsRegistry } from './editors-registry';
import {
  createDispatchEditorRootsChangeEventPlugin,
  createSyncEditorWithInputPlugin,
} from './plugins';
import {
  assignInitialDataToEditorConfig,
  assignSourceElementsToEditorConfig,
  cleanupOrphanEditorElements,
  createEditorInContext,
  isSingleRootEditor,
  loadAllEditorTranslations,
  loadEditorConstructor,
  loadEditorPlugins,
  normalizeCustomTranslations,
  queryEditablesElements,
  queryEditablesSnapshotContent,
  resolveEditorConfigElementReferences,
  resolveEditorConfigTranslations,
  setEditorEditableHeight,
  unwrapEditorContext,
  unwrapEditorWatchdog,
  wrapWithWatchdog,
} from './utils';

/**
 * The Blazor hook that manages the lifecycle of CKEditor5 instances.
 */
export class EditorComponentElement extends HTMLElement {
  /**
   * Stops observing the editor registry and immediately runs any pending cleanup.
   */
  private unmountEffect: VoidFunction | null = null;

  /**
   * Wait result for the interactive attribute.
   */
  private interactiveWait?: WaitForInteractiveResult;

  /**
   * Mounts the editor component.
   */
  async connectedCallback(): Promise<void> {
    await waitForDOMReady();

    // By default, components do not bootstrap from web components.
    // They bootstrap only when they receive the data-cke-interactive flag, which the interop sets.
    // This is a fallback for situations where CKEditor 5 is rendered on a non-interactive page.
    this.interactiveWait = waitForInteractiveAttribute(this);

    // Let's start preloading the editor constructor. We are still waiting for the interactive attribute, but
    // at least we will have the constructor ready when it arrives. It makes no difference which editor type is being
    // preloaded as they are loaded from the same package, so let's just preload the classic editor constructor.
    void loadEditorConstructor('classic');

    await this.interactiveWait.promise;
    await this.initializeEditor();
  }

  /**
   * Initializes the editor instance.
   */
  private async initializeEditor(): Promise<void> {
    const editorId = this.getAttribute('data-cke-editor-id')!;

    EditorsRegistry.the.resetErrors(editorId);

    try {
      this.style.display = 'block';

      const editor = await this.createEditor();
      const editorContext = unwrapEditorContext(editor);
      const watchdog = unwrapEditorWatchdog(editor);

      // Do not even try to broadcast about the registration of the editor
      // if hook was immediately destroyed.
      /* v8 ignore else -- @preserve */
      if (this.isConnected) {
        // Run some stuff that have to be reinitialized every-time editor is being restarted.
        const unmountDestroyWatchers = EditorsRegistry.the.mountEffect(editorId, (editor) => {
          // Enforce deregistration of the editor when it's being destroyed by watchdog.
          editor.once('destroy', () => {
            // Let's handle case when watchdog (or context watchdog) destroyed editor "externally"
            // user might also manually kill the editor using `.destroy()` method.
            // Keep pending callbacks though. Someone might register new callbacks just before calling `.destroy()`.
            EditorsRegistry.the.unregister(editorId, false);
          }, { priority: 'highest' });
        });

        this.unmountEffect = async () => {
          // If for some reason editor not fired `destroy`, enforce deregistration.
          EditorsRegistry.the.unregister(editorId);
          unmountDestroyWatchers();

          if (editorContext) {
            // If context is present, make sure it's not in unmounting phase, as it'll kill the editors.
            // If it's being destroyed, don't do anything, as the context will take care of it.
            if (editorContext.state !== 'unavailable') {
              await editorContext.context.remove(editorContext.editorContextId);
            }
          }
          else if (watchdog) {
            await watchdog.destroy();
          }
          else {
            await editor.destroy();
          }
        };

        EditorsRegistry.the.register(editorId, editor);
      }
    }
    catch (error: any) {
      /* v8 ignore start -- @preserve */
      console.error(`Error initializing CKEditor5 instance with ID "${editorId}":`, error);
      EditorsRegistry.the.error(editorId, error);
      /* v8 ignore end */
    }
  }

  /**
   * Destroys the editor instance when the component is destroyed.
   * This is important to prevent memory leaks and ensure that the editor is properly cleaned up.
   */
  async disconnectedCallback() {
    this.interactiveWait?.disconnect();
    this.style.display = 'none';
    this.unmountEffect?.();
  }

  /**
   * Creates the CKEditor instance.
   */
  private async createEditor() {
    const editorId = this.getAttribute('data-cke-editor-id')!;
    const preset = JSON.parse(this.getAttribute('data-cke-preset')!) as EditorPreset;
    const contextId = this.getAttribute('data-cke-context-id');
    const rootAttributes = JSON.parse(this.getAttribute('data-cke-root-attributes') || '{}');
    const editableHeight = this.getAttribute('data-cke-editable-height') ? Number.parseInt(this.getAttribute('data-cke-editable-height')!, 10) : null;
    const saveDebounceMs = Number.parseInt(this.getAttribute('data-cke-save-debounce-ms')!, 10);
    const language = JSON.parse(this.getAttribute('data-cke-language')!) as EditorLanguage;
    const useWatchdog = this.hasAttribute('data-cke-watchdog');
    const content = JSON.parse(this.getAttribute('data-cke-content')!) as Record<string, string>;

    const {
      customTranslations,
      editorType,
      licenseKey,
      watchdogConfig,
      config: { plugins, ...config },
    } = preset;

    const Constructor = await loadEditorConstructor(editorType);
    const context = await (
      contextId
        ? ContextsRegistry.the.waitFor(contextId)
        : null
    );

    /**
     * Builds the full editor configuration and creates the editor instance.
     */
    const buildAndCreateEditor = async () => {
      const { loadedPlugins, hasPremium } = await loadEditorPlugins(plugins);

      loadedPlugins.push(
        await createDispatchEditorRootsChangeEventPlugin({
          saveDebounceMs,
          editorId,
          targetElement: this,
        }),
      );

      if (isSingleRootEditor(editorType)) {
        loadedPlugins.push(
          await createSyncEditorWithInputPlugin(saveDebounceMs),
        );
      }

      // Mix custom translations with loaded translations.
      const loadedTranslations = await loadAllEditorTranslations(language, hasPremium);
      const mixedTranslations = [
        ...loadedTranslations,
        normalizeCustomTranslations(customTranslations || {}),
      ]
        .filter(translations => !isEmptyObject(translations));

      // Let's query all elements, and create basic configuration.
      let initialData: string | Record<string, string> = {
        ...content,
        ...queryEditablesSnapshotContent(editorId),
      };

      if (isSingleRootEditor(editorType)) {
        initialData = initialData['main'] || '';
      }

      // Depending of the editor type, and parent lookup for nearest context or initialize it without it.
      const editor = await (async () => {
        let sourceElements: HTMLElement | Record<string, HTMLElement> = queryEditablesElements(editorId);

        // Handle special case when user specified `initialData` of several root elements, but editable components
        // are not yet present in the DOM. In other words - editor is initialized before attaching root elements.
        if (!sourceElements['main']) {
          const requiredRoots = (
            isSingleRootEditor(editorType)
              ? ['main']
              : Object.keys(initialData as Record<string, string>)
          );

          if (!checkIfAllRootsArePresent(sourceElements, requiredRoots)) {
            sourceElements = await waitForAllRootsToBePresent(editorId, requiredRoots);
            initialData = {
              ...content,
              ...queryEditablesSnapshotContent(editorId),
            };
          }
        }

        // If single root editor, unwrap the element from the object.
        if (isSingleRootEditor(editorType) && 'main' in sourceElements) {
          sourceElements = sourceElements['main'];
        }

        // Do some postprocessing on received configuration.
        let resolvedConfig = {
          ...config,
          licenseKey,
          plugins: loadedPlugins,
          language,
          ...mixedTranslations.length && {
            translations: mixedTranslations,
          },
        };

        resolvedConfig = resolveEditorConfigElementReferences(resolvedConfig);
        resolvedConfig = resolveEditorConfigTranslations([...mixedTranslations].reverse(), language.ui, resolvedConfig);
        resolvedConfig = assignSourceElementsToEditorConfig(Constructor, sourceElements, resolvedConfig);
        resolvedConfig = assignInitialDataToEditorConfig(initialData, resolvedConfig);

        if (!context) {
          return Constructor.create(resolvedConfig);
        }

        const result = await createEditorInContext({
          context,
          creator: Constructor,
          config: resolvedConfig,
        });

        return result.editor;
      })();

      // Assign root attributes if they are not empty. This is needed to support custom attributes on the root element of the editor.
      if (!isEmptyObject(rootAttributes)) {
        editor.model.change((writer) => {
          writer.setAttributes(rootAttributes, editor.model.document.getRoot()!);
        });
      }

      if (isSingleRootEditor(editorType) && editableHeight) {
        setEditorEditableHeight(editor, editableHeight);
      }

      return editor;
    };

    // Do not use editor specific watchdog if context is attached, as the context is by default protected.
    if (useWatchdog && !context) {
      const watchdog = await wrapWithWatchdog(buildAndCreateEditor, watchdogConfig);

      watchdog.on('error', (_, { causesRestart }) => {
        if (causesRestart) {
          const prevEditor = EditorsRegistry.the.getItem(editorId);

          /* v8 ignore next 3 */
          if (prevEditor) {
            cleanupOrphanEditorElements(prevEditor);

            EditorsRegistry.the.unregister(editorId);
          }
        }
      });

      watchdog.on('restart', () => {
        const newInstance = watchdog.editor!;

        EditorsRegistry.the.register(editorId, newInstance);
      });

      await watchdog.create({});

      return watchdog.editor!;
    }

    return buildAndCreateEditor();
  }
}

/**
 * Checks if all required root elements are present in the elements object.
 *
 * @param elements The elements object mapping root IDs to HTMLElements.
 * @param requiredRoots The list of required root IDs.
 * @returns True if all required roots are present, false otherwise.
 */
function checkIfAllRootsArePresent(elements: Record<string, HTMLElement>, requiredRoots: string[]): boolean {
  return requiredRoots.every(rootId => elements[rootId]);
}

/**
 * Waits for all required root elements to be present in the DOM.
 *
 * @param editorId The editor's ID.
 * @param requiredRoots The list of required root IDs.
 * @returns A promise that resolves to the record of root elements.
 */
async function waitForAllRootsToBePresent(
  editorId: EditorId,
  requiredRoots: string[],
): Promise<Record<string, HTMLElement>> {
  return waitFor(
    () => {
      const elements = queryEditablesElements(editorId) as unknown as Record<string, HTMLElement>;

      if (!checkIfAllRootsArePresent(elements, requiredRoots)) {
        throw new Error(
          'It looks like not all required root elements are present yet.\n'
          + '* If you want to wait for them, ensure they are registered before editor initialization.\n'
          + '* If you want lazy initialize roots, consider removing root values from the `initialData` config '
          + 'and assign initial data in editable components.\n'
          + `Missing roots: ${requiredRoots.filter(rootId => !elements[rootId]).join(', ')}.`,
        );
      }

      return elements;
    },
    { timeOutAfter: 2000, retryAfter: 100 },
  );
}
