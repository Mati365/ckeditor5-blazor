using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Tests.Model;

public class PresetConfigTests
{
    [Fact]
    public void PresetConfig_ShouldInitializeWithDefaultValues()
    {
        var config = new PresetConfig();

        Assert.Equal(EditorType.Classic, config.EditorType);
        Assert.NotNull(config.Config);
        Assert.Empty(config.Config);
        Assert.Null(config.Cloud);
        Assert.NotNull(config.SelfHosted);
        Assert.True(config.LicenseKey.IsGPL());
        Assert.Null(config.Translations);
    }

    [Fact]
    public void WithMergedConfig_ShouldReturnNewInstanceWithMergedValues()
    {
        var initialConfig = new PresetConfig
        {
            Config = new Dictionary<string, object> { { "key1", "val1" } }
        };
        var toMerge = new Dictionary<string, object> { { "key2", "val2" } };

        var result = initialConfig.WithMergedConfig(toMerge);

        Assert.NotSame(initialConfig, result);
        Assert.Equal(2, result.Config.Count);
        Assert.Equal("val1", result.Config["key1"]);
        Assert.Equal("val2", result.Config["key2"]);
        Assert.Single(initialConfig.Config);
    }
}
