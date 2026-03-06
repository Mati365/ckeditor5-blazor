namespace CKEditor.Blazor.Model.Cloud;

/// <summary>
/// Configuration data required to import CKEditor 5 from the cloud (CDN / importmap).
/// </summary>
public sealed record CloudConfig
{
    /// <summary>
    /// The official CKEditor CDN URL used for validation.
    /// </summary>
    private static readonly string _officialCdnUrl = "https://cdn.ckeditor.com";

    /// <summary>
    /// The CKEditor 5 version to import (e.g. "36.0.0").
    /// </summary>
    public string EditorVersion { get; init; } = "47.6.0";

    /// <summary>
    /// Whether the premium package is used.
    /// </summary>
    public bool Premium { get; init; } = false;

    /// <summary>
    /// The base URL for the CKEditor CDN.
    /// Defaults to "https://cdn.ckeditor.com/".
    /// </summary>
    public string CdnUrl { get; init; } = _officialCdnUrl;

    /// <summary>
    /// CKBox information (optional).
    /// </summary>
    public CKBoxCloudConfig? CKBox { get; init; }

    /// <summary>
    /// Detects whether the CDN URL in this configuration appears to be the official CKEditor CDN.
    /// This is used to determine if certain optimizations or assumptions about the CDN structure can be made.
    /// </summary>
    /// <returns>><c>true</c> if the CDN URL appears to be the official CKEditor CDN; otherwise <c>false</c>.</returns>
    public bool HasOfficialCdn() => CdnUrl.Contains(_officialCdnUrl, StringComparison.OrdinalIgnoreCase);
}
