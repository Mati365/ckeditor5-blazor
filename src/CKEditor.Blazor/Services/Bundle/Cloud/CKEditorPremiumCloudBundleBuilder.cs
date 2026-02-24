using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;

namespace CKEditor.Blazor.Services.Bundle.Cloud;

/// <summary>
/// Generates asset package URLs for CKEditor 5 Premium Features.
/// </summary>
public class CKEditorPremiumCloudBundleBuilder : ICKEditorPremiumCloudBundleBuilder
{
    public AssetsBundle Build(string version, string cdnUrl)
    {
        var baseUrl = $"{cdnUrl}/ckeditor5-premium-features/{version}/";

        return new(
            [
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
            ],
            [
                $"{baseUrl}ckeditor5-premium-features.css"
            ]);
    }
}
