using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Tests.Model;

public class LanguageTests
{
    [Fact]
    public void Language_ShouldInitializeWithDefaultValues()
    {
        // Act
        var language = new Language();

        // Assert
        Assert.Equal("en", language.UI);
        Assert.Equal("en", language.Content);
    }

    [Fact]
    public void Language_ShouldAllowSettingValues()
    {
        // Act
        var language = new Language
        {
            UI = "pl",
            Content = "de"
        };

        // Assert
        Assert.Equal("pl", language.UI);
        Assert.Equal("de", language.Content);
    }
}
