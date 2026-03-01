using CKEditor.Blazor.Model.SelfHosted;

namespace CKEditor.Blazor.Tests.Model.SelfHosted;

public class SelfHostedConfigTests
{
    [Fact]
    public void SelfHostedConfig_ShouldInitializeCorrectly()
    {
        var config = new SelfHostedConfig();

        Assert.NotNull(config.EditorVersion);
        Assert.NotNull(config.AssetsBasePath);
        Assert.Null(config.CKBox);
    }

    [Fact]
    public void SelfHostedConfig_ShouldAllowSettingValues()
    {
        var ckboxConfig = new CKBoxSelfHostedConfig { Version = "3.0" };

        var config = new SelfHostedConfig
        {
            EditorVersion = "42.0.0",
            Premium = true,
            AssetsBasePath = "my-assets",
            CKBox = ckboxConfig
        };

        Assert.Equal("42.0.0", config.EditorVersion);
        Assert.True(config.Premium);
        Assert.Equal("my-assets", config.AssetsBasePath);
        Assert.Same(ckboxConfig, config.CKBox);
    }
}
