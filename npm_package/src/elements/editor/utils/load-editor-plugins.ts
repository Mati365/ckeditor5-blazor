import type { EditorPlugin, EditorPluginImport } from '../typings';
import type { PluginConstructor } from 'ckeditor5';

import { CKEditor5BlazorError } from '../../../ckeditor5-blazor-error';
import { CustomEditorPluginsRegistry } from '../custom-editor-plugins';

/**
 * Loads CKEditor plugins from base and premium packages.
 * First tries to load from the base 'ckeditor5' package, then falls back to 'ckeditor5-premium-features'.
 * Supports custom import descriptors ({ $import: { name, path } }) for plugins loaded from custom modules.
 *
 * @param plugins - Array of plugin names or import descriptors to load
 * @returns Promise that resolves to an array of loaded Plugin instances
 * @throws Error if a plugin is not found in either package
 */
export async function loadEditorPlugins(plugins: EditorPlugin[]): Promise<LoadedPlugins> {
  const basePackage = await import('ckeditor5');
  let premiumPackage: Record<string, any> | null = null;

  const loaders = plugins.map(async (plugin) => {
    // Handle custom import descriptor: { $import: { name, path } }
    if (isPluginImport(plugin)) {
      const { name, path } = plugin.$import;

      const mod = await import(/* @vite-ignore */ path);
      const typedMod = mod as Record<string, unknown>;
      const ctor = (Object.prototype.hasOwnProperty.call(typedMod, name) ? typedMod[name] : undefined)
        ?? (Object.prototype.hasOwnProperty.call(typedMod, 'default') ? typedMod['default'] : undefined);

      if (!ctor) {
        throw new CKEditor5BlazorError(`Plugin "${name}" not found in module "${path}".`);
      }

      return ctor as PluginConstructor;
    }

    // If the plugin is not found in the base package, try custom plugins.
    const customPlugin = await CustomEditorPluginsRegistry.the.get(plugin);

    if (customPlugin) {
      return customPlugin;
    }

    // If not found, try to load from the base package.
    const { [plugin]: basePkgImport } = basePackage as Record<string, unknown>;

    if (basePkgImport) {
      return basePkgImport as PluginConstructor;
    }

    // Plugin not found in base package, try premium package.
    /* v8 ignore start -- @preserve */
    if (!premiumPackage) {
      try {
        premiumPackage = await import('ckeditor5-premium-features');
      }
      catch (error) {
        console.error(`Failed to load premium package: ${error}`);
        throw new CKEditor5BlazorError(`Plugin "${plugin}" not found in base package and failed to load premium package.`);
      }
    }
    /* v8 ignore end */

    const { [plugin]: premiumPkgImport } = premiumPackage || {};

    if (premiumPkgImport) {
      return premiumPkgImport as PluginConstructor;
    }

    // Plugin not found in either package, throw an error.
    throw new CKEditor5BlazorError(`Plugin "${plugin}" not found in base or premium packages.`);
  });

  return {
    loadedPlugins: await Promise.all(loaders),
    hasPremium: !!premiumPackage,
  };
}

/**
 * Returns `true` when the plugin entry is an import descriptor (`{ $import: ... }`).
 */
function isPluginImport(plugin: EditorPlugin): plugin is EditorPluginImport {
  return typeof plugin === 'object' && plugin !== null && '$import' in plugin;
}

/**
 * Type representing the loaded plugins and whether premium features are available.
 */
type LoadedPlugins = {
  loadedPlugins: PluginConstructor<any>[];
  hasPremium: boolean;
};
