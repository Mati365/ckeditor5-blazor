using CKEditor.Blazor.Services;

namespace CKEditor.Blazor.Tests.Services;

public class BuildMetadataReaderTests
{
    [Fact]
    public void ResolveCKEditorVersion_ReturnsFallbackOrMetadataValue()
    {
        var version = BuildMetadataReader.ResolveCKEditorVersion();

        Assert.False(string.IsNullOrWhiteSpace(version));
        Assert.Matches(@"^\d+\.\d+\.\d+$", version);
    }

    [Fact]
    public void ResolveCKBoxVersion_ReturnsFallbackOrMetadataValue()
    {
        var version = BuildMetadataReader.ResolveCKBoxVersion();

        Assert.False(string.IsNullOrWhiteSpace(version));
        Assert.Matches(@"^\d+\.\d+\.\d+$", version);
    }

    [Fact]
    public void ResolveAssetsOutputPath_ReturnsFallbackOrMetadataValue()
    {
        var path = BuildMetadataReader.ResolveAssetsOutputPath();

        Assert.False(string.IsNullOrWhiteSpace(path));
    }

    [Fact]
    public void ResolveIncludePremiumAssets_ReturnsFallbackOrMetadataValue()
    {
        var includePremium = BuildMetadataReader.ResolveIncludePremiumAssets();

        // Can be true or false depending on compilation, simply assert it completes without throwing.
        Assert.IsType<bool>(includePremium);
    }
}
