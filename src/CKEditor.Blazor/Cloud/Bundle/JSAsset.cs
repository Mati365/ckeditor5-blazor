namespace CKEditor.Blazor.Cloud.Bundle;

/// <summary>
/// Represents a JavaScript asset in a CKEditor cloud bundle.
/// </summary>
public class JSAsset
{
    /// <summary>
    /// The name of the JavaScript asset.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The URL of the JavaScript asset.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The type of the JavaScript asset (ESM or UMD).
    /// </summary>
    public JSAssetType Type { get; set; }
}
