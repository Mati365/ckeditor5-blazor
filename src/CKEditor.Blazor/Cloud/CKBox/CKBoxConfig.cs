namespace CKEditor.Blazor.Cloud.CKBox;

/// <summary>
/// CKBox information used when importing the editor from the cloud (CDN / importmap).
/// </summary>
public class CKBoxConfig
{
    /// <summary>
    /// CKBox version (e.g. "1.2.3").
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Optional theme/skin for CKBox (e.g. "dark").
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// List of available translations.
    /// </summary>
    public List<string> Translations { get; set; } = [];

    /// <summary>
    /// The base URL for the CKBox CDN.
    /// Defaults to "https://cdn.ckbox.io/".
    /// </summary>
    public string CdnUrl { get; set; } = "https://cdn.ckbox.io";
}
