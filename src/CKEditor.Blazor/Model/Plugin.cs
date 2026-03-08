namespace CKEditor.Blazor.Model;

/// <summary>
/// Provides helpers for building CKEditor plugin configurations.
/// </summary>
/// <example>
/// <code>
/// new PresetConfig()
///     .WithPlugins("Essentials", "Bold", Plugin.Import("MyPlugin", "./my-plugin.js"));
/// </code>
/// </example>
public static class Plugin
{
    /// <summary>
    /// Creates a custom plugin descriptor that loads the named export from the given JavaScript module.
    /// </summary>
    /// <param name="name">The exported class name within the module (e.g. <c>"MyPlugin"</c>).</param>
    /// <param name="importPath">The JavaScript module path (e.g. <c>"./my-plugin.js"</c>).</param>
    /// <returns>A <see cref="PresetPluginImport"/> to pass inside <see cref="PresetConfig.WithPlugins"/>.</returns>
    public static PresetPluginImport Import(string name, string importPath) => new(name, importPath);
}
