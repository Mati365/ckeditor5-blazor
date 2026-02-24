using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;

namespace CKEditor.Blazor.Services.Bundle.Cloud;

/// <summary>
/// Builds an asset bundle for CKBox based on the provided cloud configuration.
/// </summary>
public class CKBoxCloudBundleBuilder : ICKBoxCloudBundleBuilder
{
    public AssetsBundle Build(
        string version,
        IReadOnlyList<string> translations,
        string cdnUrl,
        string theme = "lark")
    {
        var baseUrl = $"{cdnUrl.TrimEnd('/')}/ckbox/{version.Trim('/')}/";

        return new(
            [
                new() { Name = "ckbox", Url = $"{baseUrl}ckbox.js", Type = JSAssetType.UMD },
                .. translations.Select(t => new JSAsset
                {
                    Name = $"ckbox/translations/{t}",
                    Url = $"{baseUrl}translations/{t}.js",
                    Type = JSAssetType.UMD
                })
            ],
            [
                $"{baseUrl}styles/themes/{theme}.css"
            ]);
    }
}
