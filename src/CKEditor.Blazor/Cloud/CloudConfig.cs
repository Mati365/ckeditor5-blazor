using CKEditor.Blazor.Cloud.CKBox;

namespace CKEditor.Blazor.Cloud;

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
    /// List of available translations.
    /// </summary>
    public List<string> Translations { get; set; } = [];

    /// <summary>
    /// CKBox information (optional).
    /// </summary>
    public CKBoxConfig? CKBox { get; set; }
}
