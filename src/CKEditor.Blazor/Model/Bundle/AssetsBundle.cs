namespace CKEditor.Blazor.Model.Bundle;

/// <summary>
/// Represents a bundle of cloud assets (JavaScript and CSS).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AssetsBundle"/> class.
/// </remarks>
/// <param name="Js">The JavaScript assets.</param>
/// <param name="Css">The CSS asset URLs.</param>
public sealed record AssetsBundle(IReadOnlyList<JSAsset> Js, IReadOnlyList<string> Css)
{
    /// <summary>
    /// A predefined JavaScript asset for CKEditor Blazor integration, included in all bundles.
    /// </summary>
    public static readonly JSAsset BlazorIntegrationAsset = new()
    {
        Name = "ckeditor5-blazor",
        Url = "/_content/CKEditor.Blazor/ckeditor5-blazor/index.mjs",
        Type = JSAssetType.ESM
    };

    /// <summary>
    /// Creates a new bundle by merging this bundle with another one.
    /// </summary>
    /// <param name="other">The bundle to merge.</param>
    /// <returns>The merged bundle.</returns>
    public AssetsBundle Merge(AssetsBundle other) => new([.. Js, .. other.Js], [.. Css, .. other.Css]);

    /// <summary>
    /// Gets the list of ESM JavaScript URLs from this bundle.
    /// </summary>
    /// <returns>The list of distinct ESM URLs.</returns>
    public List<string> GetEsmOnlyUrls()
    {
        return [.. Js
            .Where(static asset => asset.Type == JSAssetType.ESM)
            .Select(static asset => asset.Url)
            .Distinct(StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    /// Gets the list of UMD JavaScript URLs from this bundle.
    /// </summary>
    /// <returns>The list of distinct UMD URLs.</returns>
    public List<string> GetUmdUrls()
    {
        return [.. Js
            .Where(static asset => asset.Type == JSAssetType.UMD)
            .Select(static asset => asset.Url)
            .Distinct(StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    /// Gets the list of CSS URLs from this bundle.
    /// </summary>
    /// <returns>The list of distinct CSS URLs.</returns>
    public List<string> GetCssUrls() => [.. Css.Distinct(StringComparer.OrdinalIgnoreCase)];

    /// <summary>
    /// Gets the import map for ESM modules from this bundle.
    /// </summary>
    /// <returns>A dictionary mapping module names to their URLs.</returns>
    public Dictionary<string, string> GetImportMap()
    {
        return Js
            .Where(static asset => asset.Type is JSAssetType.ESM or JSAssetType.ESM_DIRECTORY)
            .GroupBy(static asset => asset.Name, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.Last().Url,
                StringComparer.Ordinal);
    }

    /// <summary>
    /// Creates a new bundle by merging additional JavaScript assets into this bundle.
    /// </summary>
    /// <param name="js">The JavaScript assets to merge.</param>
    /// <returns>The merged bundle.</returns>
    public AssetsBundle WithMergedJs(IEnumerable<JSAsset> js) => this with { Js = [.. Js, .. js] };

    /// <summary>
    /// Creates a new bundle by merging additional CSS assets into this bundle.
    /// </summary>
    /// <param name="css">The CSS assets to merge.</param>
    /// <returns>The merged bundle.</returns>
    public AssetsBundle WithMergedCss(IEnumerable<string> css) => this with { Css = [.. Css, .. css] };
}
