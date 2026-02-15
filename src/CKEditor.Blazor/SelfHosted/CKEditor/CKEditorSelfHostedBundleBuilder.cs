using CKEditor.Blazor.Bundle;

namespace CKEditor.Blazor.SelfHosted.CKEditor;

/// <summary>
/// Generates asset URLs for self-hosted CKEditor 5 core files.
/// </summary>
public static class CKEditorSelfHostedBundleBuilder
{
    /// <summary>
    /// Creates URLs for self-hosted CKEditor 5 core JavaScript and CSS files.
    /// </summary>
    /// <param name="version">The CKEditor 5 version.</param>
    /// <param name="basePath">The base path for assets.</param>
    /// <returns>The asset bundle.</returns>
    public static AssetsBundle Build(string version, string basePath)
    {
        var baseUrl = $"{basePath.TrimEnd('/')}/ckeditor5/{version.Trim('/')}/";
        var js = new List<JSAsset>
        {
            new()
            {
                Name = "ckeditor5",
                Url = $"{baseUrl}dist/browser/ckeditor5.js",
                Type = JSAssetType.ESM
            },
            new()
            {
                Name = $"ckeditor5/translations/",
                Url = $"{baseUrl}dist/translations/",
                Type = JSAssetType.ESM_DIRECTORY
            }
        };

        var css = new List<string> { $"{baseUrl}dist/browser/ckeditor5.css" };

        return new AssetsBundle(js, css);
    }
}
