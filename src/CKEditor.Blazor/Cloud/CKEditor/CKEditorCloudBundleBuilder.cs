using CKEditor.Blazor.Cloud.Bundle;

namespace CKEditor.Blazor.Cloud.CKEditor;

/// <summary>
/// Generates asset package URLs for CKEditor 5 core files.
/// </summary>
public static class CKEditorCloudBundleBuilder
{
    private const string _cdnBaseUrl = "https://cdn.ckeditor.com/";

    /// <summary>
    /// Creates URLs for CKEditor 5 core JavaScript and CSS files.
    /// </summary>
    /// <param name="version">The CKEditor 5 version.</param>
    /// <param name="translations">List of translations.</param>
    /// <returns>The asset bundle.</returns>
    public static AssetsBundle Build(string version, IReadOnlyList<string> translations)
    {
        var baseUrl = $"{_cdnBaseUrl}ckeditor5/{version.Trim('/')}/";
        var js = new List<JSAsset>
        {
            new()
            {
                Name = "ckeditor5",
                Url = $"{baseUrl}ckeditor5.js",
                Type = JSAssetType.ESM
            }
        };

        foreach (var translation in translations)
        {
            js.Add(new JSAsset
            {
                Name = $"ckeditor5/translations/{translation}.js",
                Url = $"{baseUrl}translations/{translation}.js",
                Type = JSAssetType.ESM
            });
        }

        var css = new List<string> { $"{baseUrl}ckeditor5.css" };

        return new AssetsBundle(js, css);
    }
}
