namespace CKEditor.Blazor.Domain.Model.Bundle;

/// <summary>
/// Represents the type of JavaScript asset.
/// </summary>
public enum JSAssetType
{
    /// <summary>
    /// ECMAScript Module format.
    /// </summary>
    ESM,

    /// <summary>
    /// ECMAScript Module format with directory structure (e.g., for translations).
    /// </summary>
    ESM_DIRECTORY,

    /// <summary>
    /// Universal Module Definition format.
    /// </summary>
    UMD
}
