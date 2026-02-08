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
}
