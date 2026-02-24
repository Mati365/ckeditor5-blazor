using CKEditor.Blazor.Model.Cloud;
using CKEditor.Blazor.Model.License;
using CKEditor.Blazor.Model.SelfHosted;

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
    public Dictionary<string, string>? Translations { get; init; }

    /// <summary>
    /// Creates a new preset with the specified configuration.
    /// </summary>
    /// <param name="config">The configuration to apply.</param>
    /// <returns>A new preset with the specified configuration.</returns>
    public PresetConfig OfConfig(Dictionary<string, object> config) => this with { Config = config };

    /// <summary>
    /// Creates a new preset with merged configuration.
    /// </summary>
    /// <param name="mergeConfig">The configuration to merge.</param>
    /// <returns>A new preset with merged configuration.</returns>
    public PresetConfig OfMergedConfig(Dictionary<string, object> mergeConfig) =>
        this with {
            Config = new Dictionary<string, object>(Config.Concat(mergeConfig))
        };

    /// <summary>
    /// Creates a new preset with custom translations.
    /// </summary>
    /// <param name="translations">The custom translations to apply.</param>
    /// <returns>A new preset with custom translations.</returns>
    public PresetConfig OfCustomTranslations(Dictionary<string, string> translations) => this with { Translations = translations };

    /// <summary>
    /// Creates a new preset with the specified editor type.
    /// </summary>
    /// <param name="editorType">The editor type to use.</param>
    /// <returns>A new preset with the specified editor type.</returns>
    public PresetConfig OfEditorType(EditorType editorType) => this with { EditorType = editorType };
}
