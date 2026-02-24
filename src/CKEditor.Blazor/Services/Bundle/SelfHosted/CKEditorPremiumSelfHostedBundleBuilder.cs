using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;

namespace CKEditor.Blazor.Services.Bundle.SelfHosted;

/// <summary>
/// Generates asset URLs for self-hosted CKEditor 5 Premium Features.
/// </summary>
public class CKEditorPremiumSelfHostedBundleBuilder : ICKEditorPremiumSelfHostedBundleBuilder
{
    public AssetsBundle Build(string version, string basePath)
    {
        var baseUrl = $"{basePath.TrimEnd('/')}/ckeditor5-premium-features/{version.Trim('/')}/";

        return new(
            [
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
            ],
            [
                $"{baseUrl}dist/browser/ckeditor5-premium-features.css"
            ]);
    }
}
