using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Interfaces.Bundle;

namespace CKEditor.Blazor.Services.Bundle;

/// <summary>
/// Generates the assets bundle used to integrate CKEditor with Blazor.
/// </summary>
public class BlazorIntegrationBundleBuilder : IBlazorIntegrationBundleBuilder
{
    /// <inheritdoc />
    public AssetsBundle Build(string integrationBasePath)
    {
        var asset = new JSAsset
        {
            Name = "ckeditor5-blazor",
            Url = $"{integrationBasePath.TrimEnd('/')}/ckeditor5-blazor/index.mjs",
            Type = JSAssetType.ESM
        };

        return new([asset], []);
    }
}
