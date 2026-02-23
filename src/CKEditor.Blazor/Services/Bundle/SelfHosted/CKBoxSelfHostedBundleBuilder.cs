using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;

namespace CKEditor.Blazor.Services.Bundle.SelfHosted;

/// <summary>
/// Builds an asset bundle for self-hosted CKBox.
/// </summary>
public class CKBoxSelfHostedBundleBuilder : ICKBoxSelfHostedBundleBuilder
{
    public AssetsBundle Build(
        string version,
        IReadOnlyList<string> translations,
        string basePath,
        string theme = "lark")
    {
        var baseUrl = $"{basePath.TrimEnd('/')}/ckbox/{version.Trim('/')}/";

        var js = new List<JSAsset>
        {
            new()
            {
                Name = "ckbox",
                Url = $"{baseUrl}dist/ckbox.js",
                Type = JSAssetType.UMD
            }
        };

        foreach (var translation in translations)
        {
            js.Add(new JSAsset
            {
                Name = $"ckbox/translations/{translation}",
                Url = $"{baseUrl}dist/translations/{translation}.js",
                Type = JSAssetType.UMD
            });
        }

        var css = new List<string> { $"{baseUrl}dist/styles/{theme}.css" };

        return new AssetsBundle(js, css);
    }
}
