using CKEditor.Blazor.Bundle;

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
    /// <param name="cdnUrl">The custom CDN URL.</param>
    /// <returns>The asset bundle.</returns>
    public static AssetsBundle Build(string version, string cdnUrl)
    {
        var baseUrl = $"{cdnUrl}/ckeditor5-premium-features/{version}/";
        var js = new List<JSAsset>
        {
            new()
            {
                Name = "ckeditor5-premium-features",
                Url = $"{baseUrl}ckeditor5-premium-features.js",
                Type = JSAssetType.ESM
            },
            new()
            {
                Name = $"ckeditor5-premium-features/translations/",
                Url = $"{baseUrl}translations/",
                Type = JSAssetType.ESM_DIRECTORY
            }
        };

        var css = new List<string> { $"{baseUrl}ckeditor5-premium-features.css" };

        return new AssetsBundle(js, css);
    }
}
