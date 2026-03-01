using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Tests.Model.Bundle;

public class JSAssetTests
{
    [Fact]
    public void JSAsset_ShouldInitializeWithDefaultValues()
    {
        var asset = new JSAsset();

        Assert.Equal(string.Empty, asset.Name);
        Assert.Equal(string.Empty, asset.Url);
        Assert.Equal(JSAssetType.ESM, asset.Type);
    }

    [Fact]
    public void JSAsset_ShouldAllowSettingValues()
    {
        var asset = new JSAsset
        {
            Name = "test-asset",
            Url = "https://example.com/test.js",
            Type = JSAssetType.UMD
        };

        Assert.Equal("test-asset", asset.Name);
        Assert.Equal("https://example.com/test.js", asset.Url);
        Assert.Equal(JSAssetType.UMD, asset.Type);
    }
}
