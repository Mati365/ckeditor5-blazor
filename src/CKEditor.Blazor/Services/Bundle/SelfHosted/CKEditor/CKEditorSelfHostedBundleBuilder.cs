using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Bundle.SelfHosted;

/// <summary>
/// Generates asset URLs for self-hosted CKEditor 5 core files.
/// </summary>
public class CKEditorSelfHostedBundleBuilder : ICKEditorSelfHostedBundleBuilder
{
    public AssetsBundle Build(string version, string basePath)
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
