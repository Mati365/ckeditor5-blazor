using CKEditor.Blazor.Domain.Interfaces.Bundle.Cloud;
using CKEditor.Blazor.Domain.Model.Bundle;

namespace CKEditor.Blazor.Infrastructure.Bundle.Cloud;

/// <summary>
/// Generates asset package URLs for CKEditor 5 core files.
/// </summary>
public class CKEditorCloudBundleBuilder : ICKEditorCloudBundleBuilder
{
    public AssetsBundle Build(string version, string cdnUrl)
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
