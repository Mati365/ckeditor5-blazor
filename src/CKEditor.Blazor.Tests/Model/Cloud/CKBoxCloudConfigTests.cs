using CKEditor.Blazor.Model.Cloud;

namespace CKEditor.Blazor.Tests.Model.Cloud;

public class CKBoxCloudConfigTests
{
    [Fact]
    public void CKBoxCloudConfig_ShouldInitializeWithDefaultValues()
    {
        var config = new CKBoxCloudConfig();

        Assert.Equal(string.Empty, config.Version);
        Assert.Null(config.Theme);
        Assert.NotNull(config.Translations);
        Assert.Empty(config.Translations);
        Assert.Equal("https://cdn.ckbox.io", config.CdnUrl);
    }

    [Fact]
    public void CKBoxCloudConfig_ShouldAllowSettingValues()
    {
        var translations = new List<string> { "pl", "en" };

        var config = new CKBoxCloudConfig
        {
            Version = "1.2.3",
            Theme = "dark",
            Translations = translations,
            CdnUrl = "https://custom.ckbox.io"
        };

        Assert.Equal("1.2.3", config.Version);
        Assert.Equal("dark", config.Theme);
        Assert.Equal(translations, config.Translations);
        Assert.Equal("https://custom.ckbox.io", config.CdnUrl);
    }
}
