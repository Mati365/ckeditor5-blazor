namespace CKEditor.Blazor.Model.Cloud;

/// <summary>
/// Configuration data required to import CKEditor 5 from the cloud (CDN / importmap).
/// </summary>
public sealed record CloudConfig
{
    /// <summary>
    /// The CKEditor 5 version to import (e.g. "36.0.0").
    /// </summary>
    public string EditorVersion { get; init; } = string.Empty;

    /// <summary>
    /// Whether the premium package is used.
    /// </summary>
    public bool Premium { get; init; }

    /// <summary>
    /// The base URL for the CKEditor CDN.
    /// Defaults to "https://cdn.ckeditor.com/".
    /// </summary>
    public string CdnUrl { get; init; } = "https://cdn.ckeditor.com";

    /// <summary>
    /// CKBox information (optional).
    /// </summary>
    public CKBoxCloudConfig? CKBox { get; init; }
}
