namespace CKEditor.Blazor.Model.Bundle;

/// <summary>
/// Represents a bundle of cloud assets (JavaScript and CSS).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AssetsBundle"/> class.
/// </remarks>
/// <param name="js">The JavaScript assets.</param>
/// <param name="css">The CSS asset URLs.</param>
public class AssetsBundle(IReadOnlyList<JSAsset> js, IReadOnlyList<string> css)
{
    /// <summary>
    /// The JavaScript assets in this bundle.
    /// </summary>
    public List<JSAsset> Js { get; } = [.. js];

    /// <summary>
    /// The CSS asset URLs in this bundle.
    /// </summary>
    public List<string> Css { get; } = [.. css];

    /// <summary>
    /// Creates a new bundle by merging this bundle with another one.
    /// </summary>
    /// <param name="other">The bundle to merge.</param>
    /// <returns>The merged bundle.</returns>
    public AssetsBundle Merge(AssetsBundle other)
    {
        var js = new List<JSAsset>(Js);
        js.AddRange(other.Js);

        var css = new List<string>(Css);
        css.AddRange(other.Css);

        return new AssetsBundle(js, css);
    }

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
    public List<string> GetCssUrls()
    {
        return [.. Css.Distinct(StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    /// Gets the import map for ESM modules from this bundle.
    /// </summary>
    /// <returns>A dictionary mapping module names to their URLs.</returns>
    public Dictionary<string, string> GetImportMap()
    {
        return Js
            .Where(static asset => asset.Type == JSAssetType.ESM || asset.Type == JSAssetType.ESM_DIRECTORY)
            .GroupBy(static asset => asset.Name, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.Last().Url,
                StringComparer.Ordinal);
    }
}
