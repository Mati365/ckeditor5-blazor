namespace CKEditor.Blazor.Domain.Model.Cloud;

/// <summary>
/// Configuration data required to import CKEditor 5 from the cloud (CDN / importmap).
/// </summary>
public class CloudConfig
{
    /// <summary>
    /// The CKEditor 5 version to import (e.g. "36.0.0").
    /// </summary>
    public string EditorVersion { get; set; } = string.Empty;

    /// <summary>
    /// Whether the premium package is used.
    /// </summary>
    public bool Premium { get; set; }

    /// <summary>
    /// The base URL for the CKEditor CDN.
    /// Defaults to "https://cdn.ckeditor.com/".
    /// </summary>
    public string CdnUrl { get; set; } = "https://cdn.ckeditor.com";

    /// <summary>
    /// CKBox information (optional).
    /// </summary>
    public CKBoxCloudConfig? CKBox { get; set; }
}
