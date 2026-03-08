using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Cloud;
using CKEditor.Blazor.Model.License;
using CKEditor.Blazor.Model.SelfHosted;

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
        Assert.Null(config.CustomTranslations);
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

    [Fact]
    public void WithEditorType_ShouldReturnNewPresetWithUpdatedEditorType()
    {
        var preset = new PresetConfig().WithEditorType(EditorType.Balloon);

        Assert.Equal(EditorType.Balloon, preset.EditorType);
    }

    [Fact]
    public void WithLicenseKey_ShouldReturnNewPresetWithUpdatedLicenseKey()
    {
        var key = new LicenseKey("test-key");
        var preset = new PresetConfig().WithLicenseKey(key);

        Assert.Equal(key, preset.LicenseKey);
    }

    [Fact]
    public void WithCloud_ShouldReturnNewPresetWithUpdatedCloudConfig()
    {
        var cloud = new CloudConfig { EditorVersion = "44.0.0" };
        var preset = new PresetConfig().WithCloud(cloud);

        Assert.Same(cloud, preset.Cloud);
    }

    [Fact]
    public void WithSelfHosted_ShouldReturnNewPresetWithUpdatedSelfHostedConfig()
    {
        var selfHosted = new SelfHostedConfig { AssetsBasePath = "/assets" };
        var preset = new PresetConfig().WithSelfHosted(selfHosted);

        Assert.Same(selfHosted, preset.SelfHosted);
    }

    [Fact]
    public void WithLanguage_String_ShouldSetLanguageInConfig()
    {
        var preset = new PresetConfig().WithLanguage("pl");

        Assert.Equal("pl", preset.Config["language"]);
    }

    [Fact]
    public void WithLanguage_LanguageObject_ShouldSetLanguageRecordInConfig()
    {
        var lang = new Language { UI = "pl", Content = "en" };
        var preset = new PresetConfig().WithLanguage(lang);

        Assert.Same(lang, preset.Config["language"]);
    }

    [Fact]
    public void WithToolbar_StringItems_ShouldSetToolbarItemsInConfig()
    {
        var preset = new PresetConfig().WithToolbar("bold", "italic", "|", "undo");

        var toolbar = Assert.IsType<Dictionary<string, object>>(preset.Config["toolbar"]);
        var items = Assert.IsType<object[]>(toolbar["items"]);

        Assert.Equal(["bold", "italic", "|", "undo"], items);
    }

    [Fact]
    public void WithToolbar_WithGroupItem_ShouldSerializeGroupAsDictionary()
    {
        var preset = new PresetConfig().WithToolbar(
            "bold",
            Toolbar.Group("Text Style", "strikethrough", "superscript")
        );

        var toolbar = Assert.IsType<Dictionary<string, object>>(preset.Config["toolbar"]);
        var items = Assert.IsType<object[]>(toolbar["items"]);

        Assert.Equal(2, items.Length);
        Assert.Equal("bold", items[0]);

        var group = Assert.IsType<Dictionary<string, object>>(items[1]);
        Assert.Equal("Text Style", group["label"]);
        Assert.Equal(new[] { "strikethrough", "superscript" }, group["items"]);
    }

    [Fact]
    public void WithToolbar_ShouldNotMutateOriginalPreset()
    {
        var original = new PresetConfig();
        var updated = original.WithToolbar("bold");

        Assert.Empty(original.Config);
        Assert.NotEmpty(updated.Config);
    }

    [Fact]
    public void WithToolbarItems_String_ShouldSetRawToolbarValueInConfig()
    {
        var preset = new PresetConfig().WithToolbarItems("bold italic | undo redo");

        Assert.Equal("bold italic | undo redo", preset.Config["toolbar"]);
    }

    [Fact]
    public void WithToolbarItems_ShouldNotMutateOriginalPreset()
    {
        var original = new PresetConfig();
        var updated = original.WithToolbarItems("bold italic");

        Assert.Empty(original.Config);
        Assert.Equal("bold italic", updated.Config["toolbar"]);
    }

    [Fact]
    public void WithConfigEntry_ShouldSetConfigValue()
    {
        var preset = new PresetConfig().WithConfigEntry("placeholder", "Type here");

        Assert.Equal("Type here", preset.Config["placeholder"]);
    }

    [Fact]
    public void WithConfigEntry_ShouldOverwriteExistingConfigValueWithoutMutatingOriginalPreset()
    {
        var original = new PresetConfig
        {
            Config = new Dictionary<string, object>
            {
                ["placeholder"] = "Old value"
            }
        };

        var updated = original.WithConfigEntry("placeholder", "New value");

        Assert.Equal("Old value", original.Config["placeholder"]);
        Assert.Equal("New value", updated.Config["placeholder"]);
    }

    [Fact]
    public void WithPlugins_StringItems_ShouldSetPluginsInConfig()
    {
        var preset = new PresetConfig().WithPlugins("Essentials", "Bold");

        Assert.Equal(new object[] { "Essentials", "Bold" }, preset.Config["plugins"]);
    }

    [Fact]
    public void WithPlugins_WithImportDescriptor_ShouldIncludePluginImportInConfig()
    {
        var preset = new PresetConfig().WithPlugins("Bold", Plugin.Import("MyPlugin", "./my-plugin.js"));

        var plugins = Assert.IsType<object[]>(preset.Config["plugins"]);
        Assert.Equal(2, plugins.Length);
        Assert.Equal("Bold", plugins[0]);

        var importDescriptor = Assert.IsType<PresetPluginImport>(plugins[1]);
        Assert.Equal("MyPlugin", importDescriptor.Name);
        Assert.Equal("./my-plugin.js", importDescriptor.ImportPath);
    }

    [Fact]
    public void WithPlugins_ShouldNotMutateOriginalPreset()
    {
        var original = new PresetConfig();
        var updated = original.WithPlugins("Bold");

        Assert.Empty(original.Config);
        Assert.NotEmpty(updated.Config);
    }

    [Fact]
    public void AddPlugins_ShouldAppendToExistingPlugins()
    {
        var preset = new PresetConfig()
            .WithPlugins("Essentials", "Bold")
            .AddPlugins("Italic", "Underline");

        Assert.Equal(new object[] { "Essentials", "Bold", "Italic", "Underline" }, preset.Config["plugins"]);
    }

    [Fact]
    public void AddPlugins_WhenNoPluginsSet_ShouldBehaveAsWithPlugins()
    {
        var preset = new PresetConfig().AddPlugins("Bold", "Italic");

        Assert.Equal(new object[] { "Bold", "Italic" }, preset.Config["plugins"]);
    }

    [Fact]
    public void AddPlugins_WithImportDescriptor_ShouldAppendImportToExistingPlugins()
    {
        var importDescriptor = Plugin.Import("MyPlugin", "./my-plugin.js");

        var preset = new PresetConfig()
            .WithPlugins("Bold")
            .AddPlugins(importDescriptor);

        var plugins = Assert.IsType<object[]>(preset.Config["plugins"]);
        Assert.Equal(2, plugins.Length);
        Assert.Equal("Bold", plugins[0]);
        Assert.Equal(importDescriptor, plugins[1]);
    }

    [Fact]
    public void AddPlugins_ShouldNotMutateOriginalPreset()
    {
        var original = new PresetConfig().WithPlugins("Bold");
        var updated = original.AddPlugins("Italic");

        Assert.Equal(new object[] { "Bold" }, original.Config["plugins"]);
        Assert.Equal(new object[] { "Bold", "Italic" }, updated.Config["plugins"]);
    }

    [Fact]
    public void CombinedUsage_ShouldChainMethodsCorrectly()
    {
        var preset = new PresetConfig()
            .WithEditorType(EditorType.Inline)
            .WithPlugins("Essentials", "Bold")
            .WithToolbar("bold", Toolbar.Separator, "undo")
            .WithLanguage("pl");

        Assert.Equal(EditorType.Inline, preset.EditorType);
        Assert.Equal(new[] { "Essentials", "Bold" }, preset.Config["plugins"]);
        Assert.Equal("pl", preset.Config["language"]);

        var toolbar = Assert.IsType<Dictionary<string, object>>(preset.Config["toolbar"]);
        Assert.NotNull(toolbar["items"]);
    }

    [Fact]
    public void WithCustomTranslations_ShouldCreateTranslationsWhenNoneExist()
    {
        var preset = new PresetConfig()
            .WithCustomTranslations("pl", new Dictionary<string, string>
            {
                ["Bold"] = "Pogrubienie"
            });

        Assert.NotNull(preset.CustomTranslations);
        Assert.Equal("Pogrubienie", preset.CustomTranslations["pl"]["Bold"]);
    }

    [Fact]
    public void WithCustomTranslations_ShouldMergeIntoExistingLanguage()
    {
        var preset = new PresetConfig()
            .WithCustomTranslations("pl", new Dictionary<string, string> { ["Bold"] = "Pogrubienie" })
            .WithCustomTranslations("pl", new Dictionary<string, string> { ["Italic"] = "Kursywa" });

        Assert.Equal("Pogrubienie", preset.CustomTranslations!["pl"]["Bold"]);
        Assert.Equal("Kursywa", preset.CustomTranslations["pl"]["Italic"]);
    }

    [Fact]
    public void WithCustomTranslations_ShouldPreserveOtherLanguages()
    {
        var preset = new PresetConfig()
            .WithCustomTranslations("pl", new Dictionary<string, string> { ["Bold"] = "Pogrubienie" })
            .WithCustomTranslations("de", new Dictionary<string, string> { ["Bold"] = "Fett" });

        Assert.Equal("Pogrubienie", preset.CustomTranslations!["pl"]["Bold"]);
        Assert.Equal("Fett", preset.CustomTranslations["de"]["Bold"]);
    }

    [Fact]
    public void WithCustomTranslations_ShouldNotMutateOriginalPreset()
    {
        var original = new PresetConfig();
        var updated = original.WithCustomTranslations("pl", new Dictionary<string, string> { ["Bold"] = "Pogrubienie" });

        Assert.Null(original.CustomTranslations);
        Assert.NotNull(updated.CustomTranslations);
    }

    [Fact]
    public void ElementSelector_ShouldReturnPresetElementSelectorWithGivenSelector()
    {
        var result = PresetConfig.ElementSelector("#my-container");

        Assert.IsType<PresetElementSelector>(result);
        Assert.Equal("#my-container", result.Selector);
    }

    [Fact]
    public void TranslationReference_ShouldReturnPresetTranslationReferenceWithGivenKey()
    {
        var result = PresetConfig.TranslationReference("Bold");

        Assert.IsType<PresetTranslationReference>(result);
        Assert.Equal("Bold", result.Key);
    }
}
