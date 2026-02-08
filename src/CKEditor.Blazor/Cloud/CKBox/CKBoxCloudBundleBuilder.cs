using CKEditor.Blazor.Cloud.Bundle;

namespace CKEditor.Blazor.Cloud.CKBox;

/// <summary>
/// Builds an asset bundle for CKBox based on the provided cloud configuration.
/// </summary>
public static class CKBoxCloudBundleBuilder
{
    private const string _cdnBaseUrl = "https://cdn.ckbox.io/";

    /// <summary>
    /// Builds an asset bundle for CKBox based on the provided version, translations, and theme.
    /// </summary>
    /// <param name="version">The CKBox version.</param>
    /// <param name="translations">List of translations.</param>
    /// <param name="theme">The theme name (defaults to 'theme').</param>
    /// <returns>The asset bundle.</returns>
    public static AssetsBundle Build(string version, IReadOnlyList<string> translations, string theme = "theme")
    {
        var baseUrl = $"{_cdnBaseUrl}ckbox/{version}/";

        var js = new List<JSAsset>
        {
            new()
            {
                Name = "ckbox",
                Url = $"{baseUrl}ckbox.js",
                Type = JSAssetType.UMD
            }
        };

        foreach (var translation in translations)
        {
            js.Add(new JSAsset
            {
                Name = $"ckbox/translations/{translation}",
                Url = $"{baseUrl}translations/{translation}.js",
                Type = JSAssetType.UMD
            });
        }

        var css = new List<string> { $"{baseUrl}styles/themes/{theme}.css" };

        return new AssetsBundle(js, css);
    }
}
