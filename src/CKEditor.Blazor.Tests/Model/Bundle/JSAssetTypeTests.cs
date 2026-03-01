using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Tests.Model.Bundle;

public class JSAssetTypeTests
{
    [Fact]
    public void JSAssetType_ShouldHaveExpectedValues()
    {
        Assert.Equal(0, (int)JSAssetType.ESM);
        Assert.Equal(1, (int)JSAssetType.ESM_DIRECTORY);
        Assert.Equal(2, (int)JSAssetType.UMD);
    }
}
