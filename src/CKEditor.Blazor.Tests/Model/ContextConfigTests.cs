using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Tests.Model;

public class ContextConfigTests
{
    [Fact]
    public void ContextConfig_ShouldInitializeWithDefaultValues()
    {
        var config = new ContextConfig();

        Assert.NotNull(config.Config);
        Assert.Empty(config.Config);
        Assert.NotNull(config.Plugins);
        Assert.Empty(config.Plugins);
    }

    [Fact]
    public void ContextConfig_ShouldAllowSettingValues()
    {
        var plugins = new List<string> { "TestPlugin" };
        var dictionary = new Dictionary<string, object> { { "key", "value" } };

        var config = new ContextConfig
        {
            Config = dictionary,
            Plugins = plugins
        };

        Assert.Equal(plugins, config.Plugins);
        Assert.Equal(dictionary, config.Config);
    }
}
