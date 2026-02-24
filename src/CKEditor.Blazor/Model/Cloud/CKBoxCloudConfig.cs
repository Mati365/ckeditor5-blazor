namespace CKEditor.Blazor.Model.Cloud;

/// <summary>
/// CKBox information used when importing the editor from the cloud (CDN / importmap).
/// </summary>
public sealed record CKBoxCloudConfig
{
    /// <summary>
    /// CKBox version (e.g. "1.2.3").
    /// </summary>
    public string Version { get; init; } = string.Empty;

    /// <summary>
    /// Optional theme/skin for CKBox (e.g. "dark").
    /// </summary>
    public string? Theme { get; init; }

    /// <summary>
    /// List of available translations.
    /// </summary>
    public List<string> Translations { get; init; } = [];

    /// <summary>
    /// The base URL for the CKBox CDN.
    /// Defaults to "https://cdn.ckbox.io/".
    /// </summary>
    public string CdnUrl { get; init; } = "https://cdn.ckbox.io";
}
