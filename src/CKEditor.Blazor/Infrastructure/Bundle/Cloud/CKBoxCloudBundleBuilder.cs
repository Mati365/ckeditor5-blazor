using CKEditor.Blazor.Domain.Interfaces.Bundle.Cloud;
using CKEditor.Blazor.Domain.Model.Bundle;

namespace CKEditor.Blazor.Infrastructure.Bundle.Cloud;

/// <summary>
/// Builds an asset bundle for CKBox based on the provided cloud configuration.
/// </summary>
public class CKBoxCloudBundleBuilder : ICKBoxCloudBundleBuilder
{
    public AssetsBundle Build(
        string version,
        IReadOnlyList<string> translations,
        string cdnUrl,
        string theme = "theme")
    {
        var baseUrl = $"{cdnUrl.TrimEnd('/')}/ckbox/{version.Trim('/')}/";

        var js = new List<JSAsset>
        {
            new()
            {
                Name = "ckbox",
                Url = $"{baseUrl}ckbox.js",
                Type = JSAssetType.UMD
            }
        };

        foreach (var translation in translations)
        {
            js.Add(new JSAsset
            {
                Name = $"ckbox/translations/{translation}",
                Url = $"{baseUrl}translations/{translation}.js",
                Type = JSAssetType.UMD
            });
        }

        var css = new List<string> { $"{baseUrl}styles/themes/{theme}.css" };

        return new AssetsBundle(js, css);
    }
}
