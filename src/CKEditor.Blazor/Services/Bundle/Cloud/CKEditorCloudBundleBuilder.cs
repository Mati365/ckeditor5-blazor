using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;

namespace CKEditor.Blazor.Services.Bundle.Cloud;

/// <summary>
/// Generates asset package URLs for CKEditor 5 core files.
/// </summary>
public class CKEditorCloudBundleBuilder : ICKEditorCloudBundleBuilder
{
    public AssetsBundle Build(string version, string cdnUrl)
    {
        var baseUrl = $"{cdnUrl}/ckeditor5/{version}/";

        return new(
            [
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
            ],
            [
                $"{baseUrl}ckeditor5.css"
            ]);
    }
}
