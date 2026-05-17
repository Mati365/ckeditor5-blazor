import type { WaitForInteractiveResult } from '../../shared';
import type { EditorLanguage } from '../editor';
import type { ContextConfig } from './typings';
import type { Context, ContextWatchdog } from 'ckeditor5';

import { isEmptyObject, waitForDOMReady, waitForInteractiveAttribute } from '../../shared';
import {
  loadAllEditorTranslations,
  loadEditorPlugins,
  normalizeCustomTranslations,
  normalizeEditorLanguage,
  resolveEditorConfigElementReferences,
  resolveEditorConfigTranslations,
} from '../editor/utils';
import { ContextsRegistry } from './contexts-registry';

/**
 * The Blazor hook that mounts CKEditor context instances.
 */
export class ContextComponentElement extends HTMLElement {
  /**
   * The promise that resolves to the context instance.
   */
  private contextPromise: Promise<ContextWatchdog<Context>> | null = null;

  /**
   * Wait result for the interactive attribute.
   */
  private interactiveWait?: WaitForInteractiveResult;

  /**
   * Mounts the context component.
   */
  async connectedCallback() {
    await waitForDOMReady();

    // By default, components do not bootstrap from web components.
    // They bootstrap only when they receive the data-cke-interactive flag, which the interop sets.
    // This is a fallback for situations where CKEditor 5 is rendered on a non-interactive page.
    this.interactiveWait = waitForInteractiveAttribute(this);

    await this.interactiveWait.promise;
    await this.initializeContext();
  }

  /**
   * Initializes the context component.
   */
  private async initializeContext(): Promise<void> {
    const contextConfig = JSON.parse(this.getAttribute('data-cke-context')!) as ContextConfig;
    const { customTranslations, watchdogConfig, config: { plugins, ...config } } = contextConfig;

    const contextId = this.getAttribute('data-cke-context-id')!;
    const language = (
      this.getAttribute('data-cke-language')
        ? JSON.parse(this.getAttribute('data-cke-language')!)
        : normalizeEditorLanguage(config['language'])
    ) as EditorLanguage;

    const { loadedPlugins, hasPremium } = await loadEditorPlugins(plugins ?? []);

    // Mix custom translations with loaded translations.
    const loadedTranslations = await loadAllEditorTranslations(language, hasPremium);
    const mixedTranslations = [
      ...loadedTranslations,
      normalizeCustomTranslations(customTranslations || {}),
    ]
      .filter(translations => !isEmptyObject(translations));

    // Initialize context with watchdog.
    this.contextPromise = (async () => {
      const { ContextWatchdog, Context } = await import('ckeditor5');

      const instance = new ContextWatchdog(Context, {
        crashNumberLimit: 10,
        ...watchdogConfig,
      });

      // Construct parsed config. First resolve DOM element references in the provided configuration.
      let resolvedConfig = resolveEditorConfigElementReferences(config);

      // Then resolve translation references in the provided configuration, using the mixed translations.
      resolvedConfig = resolveEditorConfigTranslations([...mixedTranslations].reverse(), language.ui, resolvedConfig);

      await instance.create({
        ...resolvedConfig,
        language,
        plugins: loadedPlugins,
        ...mixedTranslations.length && {
          translations: mixedTranslations,
        },
      });

      instance.on('itemError', (...args) => {
        console.error('Context item error:', ...args);
      });

      return instance;
    })();

    const context = await this.contextPromise;

    if (this.isConnected) {
      ContextsRegistry.the.register(contextId, context);
    }
  }

  /**
   * Destroys the context component. Unmounts root from the editor.
   */
  async disconnectedCallback() {
    // Disconnect the observer if present.
    this.interactiveWait?.disconnect();

    const contextId = this.getAttribute('data-cke-context-id');

    // Let's hide the element during destruction to prevent flickering.
    this.style.display = 'none';

    // Let's wait for the mounted promise to resolve before proceeding with destruction.
    try {
      const context = await this.contextPromise;

      await context?.destroy();
    }
    finally {
      this.contextPromise = null;

      if (contextId && ContextsRegistry.the.hasItem(contextId)) {
        ContextsRegistry.the.unregister(contextId);
      }
    }
  }
}
