using CKEditor.Blazor.Services;

namespace CKEditor.Blazor.SelfHosted.CKBox;

/// <summary>
/// CKBox configuration for self-hosted assets.
/// </summary>
public class CKBoxSelfHostedConfig
{
    /// <summary>
    /// CKBox version (e.g. "2.8.0").
    /// Defaults to the version from build metadata.
    /// </summary>
    public string Version { get; set; } = BuildMetadataReader.ResolveCKBoxVersion();

    /// <summary>
    /// Optional theme/skin for CKBox (e.g. "lark").
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// List of available translations.
    /// </summary>
    public List<string> Translations { get; set; } = [];
}
