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

    [Fact]
    public void ExtendConfig_ShouldReturnNewInstanceWithMutatedConfig()
    {
        var original = new ContextConfig();
        var result = original.ExtendConfig(c => c["key"] = "value");

        Assert.NotSame(original, result);
        Assert.Empty(original.Config);
        Assert.Equal("value", result.Config["key"]);
    }

    [Fact]
    public void WithMergedConfig_ShouldMergeEntries()
    {
        var original = new ContextConfig { Config = new Dictionary<string, object> { ["a"] = 1 } };
        var result = original.WithMergedConfig(new Dictionary<string, object> { ["b"] = 2 });

        Assert.Equal(1, result.Config["a"]);
        Assert.Equal(2, result.Config["b"]);
        Assert.Single(original.Config);
    }

    [Fact]
    public void WithConfigEntry_ShouldAddOrOverwriteEntry()
    {
        var original = new ContextConfig { Config = new Dictionary<string, object> { ["key"] = "old" } };
        var result = original.WithConfigEntry("key", "new");

        Assert.Equal("new", result.Config["key"]);
        Assert.Equal("old", original.Config["key"]);
    }

    [Fact]
    public void WithLanguage_String_ShouldSetLanguageEntry()
    {
        var result = new ContextConfig().WithLanguage("pl");

        Assert.Equal("pl", result.Config["language"]);
    }

    [Fact]
    public void WithLanguage_Object_ShouldSetLanguageEntry()
    {
        var language = new Language { UI = "pl", Content = "en" };
        var result = new ContextConfig().WithLanguage(language);

        Assert.Equal(language, result.Config["language"]);
    }

    [Fact]
    public void WithPlugins_ShouldReplacePluginList()
    {
        var original = new ContextConfig().WithPlugins("OldPlugin");
        var result = original.WithPlugins("Essentials", "Paragraph");

        Assert.Equal(new object[] { "Essentials", "Paragraph" }, result.Config["plugins"]);
        Assert.Equal(new object[] { "OldPlugin" }, original.Config["plugins"]);
    }

    [Fact]
    public void AddPlugins_ShouldAppendToExistingList()
    {
        var original = new ContextConfig().WithPlugins("Essentials");
        var result = original.AddPlugins("Paragraph", "Bold");

        Assert.Equal(new object[] { "Essentials", "Paragraph", "Bold" }, result.Config["plugins"]);
        Assert.Equal(new object[] { "Essentials" }, original.Config["plugins"]);
    }

    [Fact]
    public void AddPlugins_WhenEmpty_ShouldBehaveLikeWithPlugins()
    {
        var result = new ContextConfig().AddPlugins("Essentials");

        Assert.Equal(new object[] { "Essentials" }, result.Config["plugins"]);
    }
}
