using CKEditor.Blazor.Services;

namespace CKEditor.Blazor.Tests.Services;

public class CKEditorOptionsTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("invalid_key", false)]
    [InlineData("GPL", true)]
    public void GetParsedLicenseKey_ReturnsExpectedResult_BasedOnDefaultLicenseKey(string? defaultKey, bool isParsedSuccessfully)
    {
        var options = new CKEditorOptions { DefaultLicenseKey = defaultKey };

        var result = options.GetParsedLicenseKey();

        if (isParsedSuccessfully)
        {
            Assert.NotNull(result);
        }
        else
        {
            Assert.Null(result);
        }
    }

    [Fact]
    public void Presets_IsInitializedToEmptyDictionary()
    {
        var options = new CKEditorOptions();

        Assert.NotNull(options.Presets);
        Assert.Empty(options.Presets);
    }
}
