using CKEditor.Blazor.Model.SelfHosted;

namespace CKEditor.Blazor.Tests.Model.SelfHosted;

public class CKBoxSelfHostedConfigTests
{
    [Fact]
    public void CKBoxSelfHostedConfig_ShouldInitializeCorrectly()
    {
        var config = new CKBoxSelfHostedConfig();

        Assert.NotNull(config.Version);
        Assert.Null(config.Theme);
        Assert.NotNull(config.Translations);
        Assert.Empty(config.Translations);
    }

    [Fact]
    public void CKBoxSelfHostedConfig_ShouldAllowSettingValues()
    {
        var translations = new List<string> { "pl" };

        var config = new CKBoxSelfHostedConfig
        {
            Version = "2.8.0-custom",
            Theme = "lark",
            Translations = translations
        };

        Assert.Equal("2.8.0-custom", config.Version);
        Assert.Equal("lark", config.Theme);
        Assert.Equal(translations, config.Translations);
    }
}
