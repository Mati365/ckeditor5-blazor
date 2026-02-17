using CKEditor.Blazor.Domain.Model.Cloud;
using CKEditor.Blazor.Domain.Model.License;
using CKEditor.Blazor.Domain.Model.SelfHosted;

namespace CKEditor.Blazor.Domain.Model;

/// <summary>
/// Represents a CKEditor preset configuration.
/// </summary>
public class PresetConfig
{
    /// <summary>
    /// The editor type for this preset.
    /// </summary>
    public EditorType EditorType { get; set; } = EditorType.Classic;

    /// <summary>
    /// The editor configuration object.
    /// </summary>
    public Dictionary<string, object> Config { get; set; } = [];

    /// <summary>
    /// Cloud configuration for this preset.
    /// </summary>
    public CloudConfig? Cloud { get; set; }

    /// <summary>
    /// Self-hosted configuration for this preset.
    /// </summary>
    public SelfHostedConfig SelfHosted { get; set; } = new SelfHostedConfig();

    /// <summary>
    /// License key for this preset.
    /// </summary>
    public LicenseKey LicenseKey { get; set; } = LicenseKey.OfGPL();

    /// <summary>
    /// Custom translations dictionary.
    /// </summary>
    public Dictionary<string, string>? Translations { get; set; }

    /// <summary>
    /// Creates a shallow copy of the preset configuration.
    /// </summary>
    /// <returns>A new preset instance with the same property values.</returns>
    public PresetConfig Clone()
    {
        return new()
        {
            EditorType = EditorType,
            Config = Config,
            Cloud = Cloud,
            SelfHosted = SelfHosted,
            LicenseKey = LicenseKey,
            Translations = Translations
        };
    }

    /// <summary>
    /// Creates a new preset with the specified configuration.
    /// </summary>
    /// <param name="config">The configuration to apply.</param>
    /// <returns>A new preset with the specified configuration.</returns>
    public PresetConfig OfConfig(Dictionary<string, object> config)
    {
        var clone = Clone();
        clone.Config = config;
        return clone;
    }

    /// <summary>
    /// Creates a new preset with merged configuration.
    /// </summary>
    /// <param name="mergeConfig">The configuration to merge.</param>
    /// <returns>A new preset with merged configuration.</returns>
    public PresetConfig OfMergedConfig(Dictionary<string, object> mergeConfig)
    {
        var newConfig = new Dictionary<string, object>(Config);

        foreach (var (key, value) in mergeConfig)
        {
            newConfig[key] = value;
        }

        var clone = Clone();
        clone.Config = newConfig;
        return clone;
    }

    /// <summary>
    /// Creates a new preset with custom translations.
    /// </summary>
    /// <param name="translations">The custom translations to apply.</param>
    /// <returns>A new preset with custom translations.</returns>
    public PresetConfig OfCustomTranslations(Dictionary<string, string> translations)
    {
        var clone = Clone();
        clone.Translations = translations;
        return clone;
    }

    /// <summary>
    /// Creates a new preset with the specified editor type.
    /// </summary>
    /// <param name="editorType">The editor type to use.</param>
    /// <returns>A new preset with the specified editor type.</returns>
    public PresetConfig OfEditorType(EditorType editorType)
    {
        var clone = Clone();
        clone.EditorType = editorType;
        return clone;
    }
}
