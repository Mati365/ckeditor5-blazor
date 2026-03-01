using CKEditor.Blazor.Model;
using CKEditor.Blazor.Serialization;

namespace CKEditor.Blazor.Tests.Serialization;

public class LanguageParserTests
{
    [Fact]
    public void Parse_Null_ShouldReturnDefaultLanguage()
    {
        var result = LanguageParser.Parse(null);

        Assert.Equal("en", result.UI);
        Assert.Equal("en", result.Content);
    }

    [Fact]
    public void Parse_StringCode_ShouldSetBothUIAndContent()
    {
        var result = LanguageParser.Parse("pl");

        Assert.Equal("pl", result.UI);
        Assert.Equal("pl", result.Content);
    }

    [Fact]
    public void Parse_LanguageInstance_ShouldReturnSameInstance()
    {
        var lang = new Language { UI = "fr", Content = "de" };

        var result = LanguageParser.Parse(lang);

        Assert.Same(lang, result);
        Assert.Equal("fr", result.UI);
        Assert.Equal("de", result.Content);
    }

    [Fact]
    public void Parse_UnknownType_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => LanguageParser.Parse(42));
    }

    [Theory]
    [InlineData("en")]
    [InlineData("de")]
    [InlineData("zh-Hans")]
    public void Parse_VariousLanguageCodes_ShouldMapBothFields(string code)
    {
        var result = LanguageParser.Parse(code);

        Assert.Equal(code, result.UI);
        Assert.Equal(code, result.Content);
    }

    [Fact]
    public void TryParse_Null_ShouldReturnTrueWithDefaultLanguage()
    {
        var success = LanguageParser.TryParse(null, out var result);

        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal("en", result!.UI);
    }

    [Fact]
    public void TryParse_ValidString_ShouldReturnTrue()
    {
        var success = LanguageParser.TryParse("pl", out var result);

        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal("pl", result!.UI);
    }

    [Fact]
    public void TryParse_InvalidType_ShouldReturnFalse()
    {
        var success = LanguageParser.TryParse(3.14, out var result);

        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void TryParse_LanguageInstance_ShouldReturnTrue()
    {
        var lang = new Language { UI = "it", Content = "it" };

        var success = LanguageParser.TryParse(lang, out var result);

        Assert.True(success);
        Assert.Same(lang, result);
    }
}
