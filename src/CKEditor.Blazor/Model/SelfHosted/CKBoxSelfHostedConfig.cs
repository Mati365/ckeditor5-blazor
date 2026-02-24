using CKEditor.Blazor.Services;

namespace CKEditor.Blazor.Model.SelfHosted;

/// <summary>
/// CKBox configuration for self-hosted assets.
/// </summary>
public sealed record CKBoxSelfHostedConfig
{
    /// <summary>
    /// CKBox version (e.g. "2.8.0").
    /// Defaults to the version from build metadata.
    /// </summary>
    public string Version { get; init; } = BuildMetadataReader.ResolveCKBoxVersion();

    /// <summary>
    /// Optional theme/skin for CKBox (e.g. "lark").
    /// </summary>
    public string? Theme { get; init; }

    /// <summary>
    /// List of available translations.
    /// </summary>
    public List<string> Translations { get; init; } = [];
}
