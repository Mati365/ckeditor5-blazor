using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Bundle;

namespace CKEditor.Blazor.Tests.Services.Bundle;

public class BlazorIntegrationAssetFactoryTests
{
    [Fact]
    public void Create_ShouldReturnCorrectBlazorIntegrationAssetsBundle()
    {
        var factory = new BlazorIntegrationBundleBuilder();

        var bundle = factory.Build("/_content/CKEditor.Blazor");

        Assert.Single(bundle.Js);
        var asset = bundle.Js[0];

        Assert.Equal("ckeditor5-blazor", asset.Name);
        Assert.Equal("/_content/CKEditor.Blazor/ckeditor5-blazor/index.mjs", asset.Url);
        Assert.Equal(JSAssetType.ESM, asset.Type);
        Assert.Empty(bundle.Css);
    }
}
