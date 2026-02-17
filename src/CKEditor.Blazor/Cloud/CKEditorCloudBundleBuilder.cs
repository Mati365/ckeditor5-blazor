using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Cloud;

/// <summary>
/// Generates asset package URLs for CKEditor 5 core files.
/// </summary>
public static class CKEditorCloudBundleBuilder
{
    /// <summary>
    /// Creates URLs for CKEditor 5 core JavaScript and CSS files.
    /// </summary>
    /// <param name="version">The CKEditor 5 version.</param>
    /// <param name="cdnUrl">The custom CDN URL.</param>
    /// <returns>The asset bundle.</returns>
    public static AssetsBundle Build(string version, string cdnUrl)
    {
        var baseUrl = $"{cdnUrl}/ckeditor5/{version}/";
        var js = new List<JSAsset>
        {
            new()
            {
                Name = "ckeditor5",
                Url = $"{baseUrl}ckeditor5.js",
                Type = JSAssetType.ESM
            },
            new()
            {
                Name = $"ckeditor5/translations/",
                Url = $"{baseUrl}translations/",
                Type = JSAssetType.ESM_DIRECTORY
            }
        };

        var css = new List<string> { $"{baseUrl}ckeditor5.css" };

        return new AssetsBundle(js, css);
    }
}
