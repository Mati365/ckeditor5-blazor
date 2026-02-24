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

        return new(
            [
                new() { Name = "ckbox", Url = $"{baseUrl}dist/ckbox.js", Type = JSAssetType.UMD },
                .. translations.Select(t => new JSAsset
                {
                    Name = $"ckbox/translations/{t}",
                    Url = $"{baseUrl}dist/translations/{t}.js",
                    Type = JSAssetType.UMD
                })
            ],
            [
                $"{baseUrl}dist/styles/{theme}.css"
            ]);
    }
}
