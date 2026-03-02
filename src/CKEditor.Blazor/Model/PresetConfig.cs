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
}
