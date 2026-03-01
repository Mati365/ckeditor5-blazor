using CKEditor.Blazor.Model.Cloud;

namespace CKEditor.Blazor.Tests.Model.Cloud;

public class CloudConfigTests
{
    [Fact]
    public void CloudConfig_ShouldInitializeWithDefaultValues()
    {
        // Act
        var config = new CloudConfig();

        // Assert
        Assert.Equal("47.3.0", config.EditorVersion);
        Assert.False(config.Premium);
        Assert.Equal("https://cdn.ckeditor.com", config.CdnUrl);
        Assert.Null(config.CKBox);
    }

    [Fact]
    public void CloudConfig_ShouldAllowSettingValues()
    {
        // Arrange
        var ckboxConfig = new CKBoxCloudConfig { Version = "2.0" };

        // Act
        var config = new CloudConfig
        {
            EditorVersion = "40.0.0",
            Premium = true,
            CdnUrl = "https://custom.cdn.com",
            CKBox = ckboxConfig
        };

        // Assert
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
        // Arrange
        var config = new CloudConfig { CdnUrl = url };

        // Act
        var result = config.HasOfficialCdn();

        // Assert
        Assert.Equal(expected, result);
    }
}
