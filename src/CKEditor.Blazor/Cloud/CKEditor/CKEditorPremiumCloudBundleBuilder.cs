using CKEditor.Blazor.Cloud.Bundle;

namespace CKEditor.Blazor.Cloud.CKEditor;

/// <summary>
/// Generates asset package URLs for CKEditor 5 Premium Features.
/// </summary>
public static class CKEditorPremiumCloudBundleBuilder
{
    /// <summary>
    /// Creates URLs for CKEditor 5 Premium Features JavaScript and CSS files.
    /// </summary>
    /// <param name="version">The CKEditor 5 version.</param>
    /// <param name="translations">List of translations.</param>
    /// <param name="cdnUrl">The custom CDN URL.</param>
    /// <returns>The asset bundle.</returns>
    public static AssetsBundle Build(string version, IReadOnlyList<string> translations, string cdnUrl)
    {
        var baseUrl = $"{cdnUrl.TrimEnd('/')}/ckeditor5-premium-features/{version.Trim('/')}/";
        var js = new List<JSAsset>
        {
            new()
            {
                Name = "ckeditor5-premium-features",
                Url = $"{baseUrl}ckeditor5-premium-features.js",
                Type = JSAssetType.ESM
            }
        };

        foreach (var translation in translations)
        {
            js.Add(new()
            {
                Name = $"ckeditor5-premium-features/translations/{translation}.js",
                Url = $"{baseUrl}translations/{translation}.js",
                Type = JSAssetType.ESM
            });
        }

        var css = new List<string> { $"{baseUrl}ckeditor5-premium-features.css" };

        return new AssetsBundle(js, css);
    }
}
