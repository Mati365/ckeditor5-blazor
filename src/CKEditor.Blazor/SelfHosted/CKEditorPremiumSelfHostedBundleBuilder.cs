using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.SelfHosted;

/// <summary>
/// Generates asset URLs for self-hosted CKEditor 5 Premium Features.
/// </summary>
public static class CKEditorPremiumSelfHostedBundleBuilder
{
    /// <summary>
    /// Creates URLs for self-hosted CKEditor 5 Premium Features JavaScript and CSS files.
    /// </summary>
    /// <param name="version">The CKEditor 5 version.</param>
    /// <param name="basePath">The base path for assets.</param>
    /// <returns>The asset bundle.</returns>
    public static AssetsBundle Build(string version, string basePath)
    {
        var baseUrl = $"{basePath.TrimEnd('/')}/ckeditor5-premium-features/{version.Trim('/')}/";
        var js = new List<JSAsset>
        {
            new()
            {
                Name = "ckeditor5-premium-features",
                Url = $"{baseUrl}dist/browser/ckeditor5-premium-features.js",
                Type = JSAssetType.ESM
            },
            new()
            {
                Name = $"ckeditor5-premium-features/translations/",
                Url = $"{baseUrl}dist/translations/",
                Type = JSAssetType.ESM_DIRECTORY
            }
        };

        var css = new List<string> { $"{baseUrl}dist/browser/ckeditor5-premium-features.css" };

        return new AssetsBundle(js, css);
    }
}
