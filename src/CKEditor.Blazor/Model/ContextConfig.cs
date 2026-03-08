namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents a CKEditor context configuration.
/// </summary>
public sealed record ContextConfig
{
    /// <summary>
    /// The context configuration object.
    /// </summary>
    public Dictionary<string, object> Config { get; init; } = [];

    /// <summary>
    /// Plugins to be loaded in the context.
    /// </summary>
    public List<string> Plugins { get; init; } = [];

    /// <summary>
    /// Creates a new context config after applying <paramref name="extendAction"/> to a copy of the current config.
    /// </summary>
    /// <param name="extendAction">An action that mutates the copied config dictionary.</param>
    /// <returns>A new context config with the extended configuration.</returns>
    public ContextConfig ExtendConfig(Action<Dictionary<string, object>> extendAction)
    {
        var newConfig = new Dictionary<string, object>(Config);

        extendAction(newConfig);

        return this with { Config = newConfig };
    }

    /// <summary>
    /// Creates a new context config with merged configuration.
    /// </summary>
    /// <param name="mergeConfig">The configuration to merge (shallow).</param>
    /// <returns>A new context config with merged configuration.</returns>
    public ContextConfig WithMergedConfig(Dictionary<string, object> mergeConfig) =>
        this with
        {
            Config = new Dictionary<string, object>(Config.Concat(mergeConfig))
        };

    /// <summary>
    /// Creates a new context config with a single config entry added or overwritten.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <param name="value">The configuration value.</param>
    /// <returns>A new context config with the configuration entry applied.</returns>
    public ContextConfig WithConfigEntry(string key, object value) => ExtendConfig(config => config[key] = value);

    /// <summary>
    /// Creates a new context config with the UI language set to <paramref name="language"/>.
    /// </summary>
    /// <param name="language">The BCP 47 language tag (e.g. <c>"pl"</c>, <c>"en"</c>).</param>
    /// <returns>A new context config with the language configuration applied.</returns>
    public ContextConfig WithLanguage(string language) => ExtendConfig(config => config["language"] = language);

    /// <summary>
    /// Creates a new context config with separate UI and content locales.
    /// </summary>
    /// <param name="language">A <see cref="Language"/> instance with <c>UI</c> and <c>Content</c> locale codes.</param>
    /// <returns>A new context config with the language configuration applied.</returns>
    public ContextConfig WithLanguage(Language language) => ExtendConfig(config => config["language"] = language);

    /// <summary>
    /// Creates a new context config with the plugins set to <paramref name="plugins"/>.
    /// Accepts string plugin names (e.g. <c>"Bold"</c>) as well as <see cref="PresetPluginImport"/>
    /// instances created via <see cref="Plugin.Import"/> for loading plugins from custom module paths.
    /// </summary>
    /// <param name="plugins">
    /// The plugins to activate: string names for built-in plugins, or <see cref="PresetPluginImport"/>
    /// instances for plugins loaded from a custom JavaScript module path.
    /// </param>
    /// <returns>A new context config with the plugins configuration applied.</returns>
    /// <example>
    /// <code>
    /// config.WithPlugins("Essentials", "Bold", Plugin.Import("MyPlugin", "./my-plugin.js"));
    /// </code>
    /// </example>
    public ContextConfig WithPlugins(params object[] plugins) => ExtendConfig(config => config["plugins"] = plugins);

    /// <summary>
    /// Appends <paramref name="plugins"/> to the existing plugin list.
    /// If no plugins have been set yet, behaves identically to <see cref="WithPlugins"/>.
    /// Accepts string plugin names (e.g. <c>"Bold"</c>) as well as <see cref="PresetPluginImport"/>
    /// instances created via <see cref="Plugin.Import"/>.
    /// </summary>
    /// <param name="plugins">The plugins to append.</param>
    /// <returns>A new context config with the combined plugin list.</returns>
    /// <example>
    /// <code>
    /// config
    ///     .WithPlugins("Essentials", "Bold")
    ///     .AddPlugins("Italic", Plugin.Import("MyPlugin", "./my-plugin.js"));
    /// </code>
    /// </example>
    public ContextConfig AddPlugins(params object[] plugins) =>
        ExtendConfig(config =>
        {
            var existing = config.TryGetValue("plugins", out var raw) && raw is object[] arr
                ? arr
                : [];

            config["plugins"] = existing.Concat(plugins).ToArray();
        });
}
