using CKEditor.Blazor.Model.Cloud;
using CKEditor.Blazor.Model.License;
using CKEditor.Blazor.Model.SelfHosted;
using CKEditor.Blazor.Serialization;

namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents a CKEditor preset configuration.
/// </summary>
public sealed record PresetConfig
{
    /// <summary>
    /// The editor type for this preset.
    /// </summary>
    public EditorType EditorType { get; init; } = EditorType.Classic;

    /// <summary>
    /// The editor configuration object.
    /// </summary>
    public Dictionary<string, object> Config { get; init; } = [];

    /// <summary>
    /// The watchdog configuration object.
    /// </summary>
    public Dictionary<string, object> WatchdogConfig { get; init; } = [];

    /// <summary>
    /// Cloud configuration for this preset.
    /// </summary>
    public CloudConfig? Cloud { get; init; }

    /// <summary>
    /// Self-hosted configuration for this preset.
    /// </summary>
    public SelfHostedConfig SelfHosted { get; init; } = new SelfHostedConfig();

    /// <summary>
    /// License key for this preset.
    /// </summary>
    public LicenseKey LicenseKey { get; init; } = LicenseKey.OfGPL();

    /// <summary>
    /// Custom translations dictionary.
    /// </summary>
    public EditorTranslations? CustomTranslations { get; init; }

    /// <summary>
    /// Creates a new preset with merged configuration.
    /// </summary>
    /// <param name="mergeConfig">The configuration to merge.</param>
    /// <returns>A new preset with merged configuration.</returns>
    public PresetConfig WithMergedConfig(Dictionary<string, object> mergeConfig) =>
        this with
        {
            Config = new Dictionary<string, object>(Config.Concat(mergeConfig))
        };

    /// <summary>
    /// Creates a new preset after applying <paramref name="extendAction"/> to a copy of the current config.
    /// </summary>
    /// <param name="extendAction">An action that mutates the copied config dictionary.</param>
    /// <returns>A new preset with the extended configuration.</returns>
    public PresetConfig ExtendConfig(Action<Dictionary<string, object>> extendAction)
    {
        var newConfig = new Dictionary<string, object>(Config);

        extendAction(newConfig);

        return this with { Config = newConfig };
    }

    /// <summary>
    /// Creates a new preset with provided watchdog configuration.
    /// </summary>
    /// <param name="watchdogConfig">Watchdog config to be assigned to preset.</param>
    /// <returns>A new preset with the extended watchdog configuration.</returns>
    public PresetConfig WithWatchdogConfig(Dictionary<string, object> watchdogConfig) =>
        this with
        {
            WatchdogConfig = watchdogConfig
        };

    /// <summary>
    /// Creates a new preset with the specified toolbar items. Supports string item names,
    /// <c>"|"</c> separators (or <see cref="Toolbar.Separator"/>), and <see cref="ToolbarGroupItem"/>
    /// instances created via <see cref="Toolbar.Group"/>.
    /// </summary>
    /// <param name="items">
    /// The toolbar items: strings, <c>"|"</c> separators, or <see cref="ToolbarGroupItem"/> groups.
    /// </param>
    /// <returns>A new preset with the toolbar configuration applied.</returns>
    /// <example>
    /// <code>
    /// preset.WithToolbar(
    ///     "bold", "italic",
    ///     Toolbar.Separator,
    ///     Toolbar.Group("Text Style", "strikethrough", "superscript"),
    ///     Toolbar.Separator,
    ///     "undo", "redo"
    /// );
    /// </code>
    /// </example>
    public PresetConfig WithToolbar(params object[] items) =>
        ExtendConfig(config => config["toolbar"] = new Dictionary<string, object>
        {
            ["items"] = items
                .Select(item => item is ToolbarGroupItem group
                    ? new Dictionary<string, object>
                    {
                        ["label"] = group.Label,
                        ["items"] = group.Items.ToArray()
                    }
                    : item)
                .ToArray()
        });

    /// <summary>
    /// Creates a new preset with the UI language set to <paramref name="language"/>.
    /// </summary>
    /// <param name="language">The BCP 47 language tag (e.g. <c>"pl"</c>, <c>"en"</c>).</param>
    /// <returns>A new preset with the language configuration applied.</returns>
    public PresetConfig WithLanguage(string language) => ExtendConfig(config => config["language"] = language);

    /// <summary>
    /// Creates a new preset with separate UI and content locales.
    /// </summary>
    /// <param name="language">A <see cref="Language"/> instance with <c>UI</c> and <c>Content</c> locale codes.</param>
    /// <returns>A new preset with the language configuration applied.</returns>
    public PresetConfig WithLanguage(Language language) => ExtendConfig(config => config["language"] = language);

    /// <summary>
    /// Creates a new preset with the editor plugins set to <paramref name="plugins"/>.
    /// Accepts string plugin names (e.g. <c>"Bold"</c>) as well as <see cref="PresetPluginImport"/>
    /// instances created via <see cref="Plugin.Import"/> for loading plugins from custom module paths.
    /// </summary>
    /// <param name="plugins">
    /// The plugins to activate: string names for built-in plugins, or <see cref="PresetPluginImport"/>
    /// instances for plugins loaded from a custom JavaScript module path.
    /// </param>
    /// <returns>A new preset with the plugins configuration applied.</returns>
    /// <example>
    /// <code>
    /// preset.WithPlugins("Essentials", "Bold", Plugin.Import("MyPlugin", "./my-plugin.js"));
    /// </code>
    /// </example>
    public PresetConfig WithPlugins(params object[] plugins) => ExtendConfig(config => config["plugins"] = plugins);

    /// <summary>
    /// Appends <paramref name="plugins"/> to the existing plugin list.
    /// If no plugins have been set yet, behaves identically to <see cref="WithPlugins"/>.
    /// Accepts string plugin names (e.g. <c>"Bold"</c>) as well as <see cref="PresetPluginImport"/>
    /// instances created via <see cref="Plugin.Import"/>.
    /// </summary>
    /// <param name="plugins">The plugins to append.</param>
    /// <returns>A new preset with the combined plugin list.</returns>
    /// <example>
    /// <code>
    /// preset
    ///     .WithPlugins("Essentials", "Bold")
    ///     .AddPlugins("Italic", Plugin.Import("MyPlugin", "./my-plugin.js"));
    /// </code>
    /// </example>
    public PresetConfig AddPlugins(params object[] plugins) =>
        ExtendConfig(config =>
        {
            var existing = config.TryGetValue("plugins", out var raw) && raw is object[] arr
                ? arr
                : [];

            config["plugins"] = existing.Concat(plugins).ToArray();
        });

    /// <summary>
    /// Creates a new preset with a single config entry added or overwritten.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <param name="value">The configuration value.</param>
    /// <returns>A new preset with the configuration entry applied.</returns>
    public PresetConfig WithConfigEntry(string key, object value) => ExtendConfig(config => config[key] = value);

    /// <summary>
    /// Creates a new preset with the specified editor type.
    /// </summary>
    /// <param name="editorType">The editor type to use.</param>
    /// <returns>A new preset with the editor type applied.</returns>
    public PresetConfig WithEditorType(EditorType editorType) => this with { EditorType = editorType };

    /// <summary>
    /// Creates a new preset with the specified license key.
    /// </summary>
    /// <param name="licenseKey">The license key to use.</param>
    /// <returns>A new preset with the license key applied.</returns>
    public PresetConfig WithLicenseKey(LicenseKey licenseKey) => this with { LicenseKey = licenseKey };

    /// <summary>
    /// Creates a new preset with the specified license key string parsed into a <see cref="LicenseKey"/>.
    /// </summary>
    /// <param name="licenseKey">The license key string to parse.</param>
    /// <returns>A new preset with the license key applied.</returns>
    public PresetConfig WithLicenseKey(string licenseKey) => this with { LicenseKey = LicenseKeyParser.Parse(licenseKey) };

    /// <summary>
    /// Creates a new preset with the specified cloud configuration.
    /// </summary>
    /// <param name="cloud">The cloud configuration to use.</param>
    /// <returns>A new preset with the cloud configuration applied.</returns>
    public PresetConfig WithCloud(CloudConfig cloud) => this with { Cloud = cloud };

    /// <summary>
    /// Creates a new preset with the specified self-hosted configuration.
    /// </summary>
    /// <param name="selfHosted">The self-hosted configuration to use.</param>
    /// <returns>A new preset with the self-hosted configuration applied.</returns>
    public PresetConfig WithSelfHosted(SelfHostedConfig selfHosted) => this with { SelfHosted = selfHosted };

    /// <summary>
    /// Creates a new preset with custom translations for the specified language merged into
    /// the existing <see cref="CustomTranslations"/> dictionary.
    /// If no translations exist yet, a new <see cref="EditorTranslations"/> instance is created.
    /// </summary>
    /// <param name="language">The BCP 47 language code (e.g. <c>"pl"</c>, <c>"de"</c>).</param>
    /// <param name="translations">A map of original UI strings to their translated equivalents.</param>
    /// <returns>A new preset with the translations applied.</returns>
    public PresetConfig WithCustomTranslations(string language, Dictionary<string, string> translations)
    {
        var existing = CustomTranslations ?? [];
        var merged = new EditorTranslations();

        foreach (var (lang, dict) in existing)
        {
            merged[lang] = dict;
        }

        merged[language] = existing.TryGetValue(language, out var current)
            ? new Dictionary<string, string>(current.Concat(translations))
            : translations;

        return this with { CustomTranslations = merged };
    }

    /// <summary>
    /// Creates a DOM element reference that resolves the given CSS selector to an <c>HTMLElement</c>
    /// during editor initialization.
    /// Serialized as <c>{ "$element": "selector" }</c>.
    /// </summary>
    /// <param name="selector">A CSS selector string (e.g. <c>"#my-container"</c>).</param>
    /// <returns>A <see cref="PresetElementSelector"/> to pass inside any config entry.</returns>
    public static PresetElementSelector ElementSelector(string selector) => new(selector);

    /// <summary>
    /// Creates a translation key reference that is resolved to the matching localized string
    /// during editor initialization.
    /// Serialized as <c>{ "$translation": "key" }</c>.
    /// </summary>
    /// <param name="key">The translation key (e.g. <c>"Bold"</c>).</param>
    /// <returns>A <see cref="PresetTranslationReference"/> to pass inside any config entry.</returns>
    public static PresetTranslationReference TranslationReference(string key) => new(key);
}
