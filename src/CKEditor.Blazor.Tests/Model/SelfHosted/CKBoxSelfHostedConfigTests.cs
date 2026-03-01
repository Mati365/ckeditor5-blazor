using CKEditor.Blazor.Model.SelfHosted;

namespace CKEditor.Blazor.Tests.Model.SelfHosted;

public class CKBoxSelfHostedConfigTests
{
    [Fact]
    public void CKBoxSelfHostedConfig_ShouldInitializeCorrectly()
    {
        // Act
        var config = new CKBoxSelfHostedConfig();

        // Assert
        Assert.NotNull(config.Version);
        Assert.Null(config.Theme);
        Assert.NotNull(config.Translations);
        Assert.Empty(config.Translations);
    }

    [Fact]
    public void CKBoxSelfHostedConfig_ShouldAllowSettingValues()
    {
        // Arrange
        var translations = new List<string> { "pl" };

        // Act
        var config = new CKBoxSelfHostedConfig
        {
            Version = "2.8.0-custom",
            Theme = "lark",
            Translations = translations
        };

        // Assert
        Assert.Equal("2.8.0-custom", config.Version);
        Assert.Equal("lark", config.Theme);
        Assert.Equal(translations, config.Translations);
    }
}
