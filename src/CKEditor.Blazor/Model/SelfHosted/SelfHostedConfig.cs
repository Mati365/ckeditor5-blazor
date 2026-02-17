using CKEditor.Blazor.Services;

namespace CKEditor.Blazor.Model.SelfHosted;

/// <summary>
/// Configuration data for self-hosted CKEditor 5 assets.
/// </summary>
public class SelfHostedConfig
{
    /// <summary>
    /// The CKEditor 5 version to use (e.g. "47.3.0").
    /// Defaults to the version from build metadata.
    /// </summary>
    public string EditorVersion { get; set; } = BuildMetadataReader.ResolveCKEditorVersion();

    /// <summary>
    /// Whether to include premium features.
    /// Defaults to the value from build metadata.
    /// </summary>
    public bool Premium { get; set; } = BuildMetadataReader.ResolveIncludePremiumAssets();

    /// <summary>
    /// The base path for CKEditor assets (relative to wwwroot).
    /// Defaults to the value from build metadata.
    /// </summary>
    public string AssetsBasePath { get; set; } = BuildMetadataReader.ResolveAssetsOutputPath();

    /// <summary>
    /// CKBox information (optional).
    /// </summary>
    public CKBoxSelfHostedConfig? CKBox { get; set; }
}
