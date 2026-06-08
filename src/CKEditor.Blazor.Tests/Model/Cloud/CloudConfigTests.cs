using CKEditor.Blazor.Model.Cloud;

namespace CKEditor.Blazor.Tests.Model.Cloud;

public class CloudConfigTests
{
    [Fact]
    public void CloudConfig_ShouldInitializeWithDefaultValues()
    {
        var config = new CloudConfig();

        Assert.Equal("48.2.0", config.EditorVersion);
        Assert.False(config.Premium);
        Assert.Equal("https://cdn.ckeditor.com", config.CdnUrl);
        Assert.Null(config.CKBox);
    }

    [Fact]
    public void CloudConfig_ShouldAllowSettingValues()
    {
        var ckboxConfig = new CKBoxCloudConfig { Version = "2.0" };

        var config = new CloudConfig
        {
            EditorVersion = "40.0.0",
            Premium = true,
            CdnUrl = "https://custom.cdn.com",
            CKBox = ckboxConfig
        };

        Assert.Equal("40.0.0", config.EditorVersion);
        Assert.True(config.Premium);
        Assert.Equal("https://custom.cdn.com", config.CdnUrl);
        Assert.Same(ckboxConfig, config.CKBox);
    }

    [Theory]
    [InlineData("https://cdn.ckeditor.com", true)]
    [InlineData("https://cdn.ckeditor.com/", true)]
    [InlineData("HTTPS://CDN.CKEDITOR.COM", true)]
    [InlineData("https://custom.cdn.com", false)]
    public void HasOfficialCdn_ShouldReturnExpectedResult(string url, bool expected)
    {
        var config = new CloudConfig { CdnUrl = url };

        var result = config.HasOfficialCdn();

        Assert.Equal(expected, result);
    }
}
