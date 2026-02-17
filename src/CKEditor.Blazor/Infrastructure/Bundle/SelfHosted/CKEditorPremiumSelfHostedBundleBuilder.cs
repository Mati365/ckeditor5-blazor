using CKEditor.Blazor.Domain.Interfaces.Bundle.SelfHosted;
using CKEditor.Blazor.Domain.Model.Bundle;

namespace CKEditor.Blazor.Infrastructure.Bundle.SelfHosted;

/// <summary>
/// Generates asset URLs for self-hosted CKEditor 5 Premium Features.
/// </summary>
public class CKEditorPremiumSelfHostedBundleBuilder : ICKEditorPremiumSelfHostedBundleBuilder
{
    public AssetsBundle Build(string version, string basePath)
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
