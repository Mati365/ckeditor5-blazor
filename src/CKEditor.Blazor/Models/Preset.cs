namespace CKEditor.Blazor.Models;

/// <summary>
/// Represents a CKEditor preset configuration.
/// </summary>
public class Preset
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
    /// Custom translations dictionary.
    /// </summary>
    public Dictionary<string, string>? Translations { get; set; }

    /// <summary>
    /// Creates a new preset with the specified configuration.
    /// </summary>
    /// <param name="config">The configuration to apply.</param>
    /// <returns>A new preset with the specified configuration.</returns>
    public Preset OfConfig(Dictionary<string, object> config)
    {
        return new Preset
        {
            EditorType = EditorType,
            Config = config,
            Cloud = Cloud,
            Translations = Translations
        };
    }

    /// <summary>
    /// Creates a new preset with merged configuration.
    /// </summary>
    /// <param name="mergeConfig">The configuration to merge.</param>
    /// <returns>A new preset with merged configuration.</returns>
    public Preset OfMergedConfig(Dictionary<string, object> mergeConfig)
    {
        var newConfig = new Dictionary<string, object>(Config);

        foreach (var (key, value) in mergeConfig)
        {
            newConfig[key] = value;
        }

        return new Preset
        {
            EditorType = EditorType,
            Config = newConfig,
            Cloud = Cloud,
            Translations = Translations
        };
    }

    /// <summary>
    /// Creates a new preset with custom translations.
    /// </summary>
    /// <param name="translations">The custom translations to apply.</param>
    /// <returns>A new preset with custom translations.</returns>
    public Preset OfCustomTranslations(Dictionary<string, string> translations)
    {
        return new Preset
        {
            EditorType = EditorType,
            Config = Config,
            Cloud = Cloud,
            Translations = translations
        };
    }

    /// <summary>
    /// Creates a new preset with the specified editor type.
    /// </summary>
    /// <param name="editorType">The editor type to use.</param>
    /// <returns>A new preset with the specified editor type.</returns>
    public Preset OfEditorType(EditorType editorType)
    {
        return new Preset
        {
            EditorType = editorType,
            Config = Config,
            Cloud = Cloud,
            Translations = Translations
        };
    }
}
