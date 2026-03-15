using CKEditor.Blazor.Exceptions;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Serialization;
using CKEditor.Blazor.Services;
using CKEditor.Blazor.Tests.Helpers;
using Microsoft.Extensions.Options;

namespace CKEditor.Blazor.Tests.Services;

public class ConfigManagerTests
{
    private static IOptions<CKEditorOptions> CreateOptions(string? defaultKey = null)
    {
        return Options.Create(new CKEditorOptions
        {
            DefaultLicenseKey = defaultKey
        });
    }

    [Fact]
    public void Constructor_RegistersDefaultPresetAndContext()
    {
        var manager = new ConfigManager(CreateOptions("GPL"));

        Assert.True(manager.GetPresets().ContainsKey("default"));
        Assert.True(manager.GetContexts().ContainsKey("default"));
    }

    [Fact]
    public void Constructor_UsesDefaultLicenseKeyForAllPresets()
    {
        var jwt = JwtTestHelper.BuildValid("sh");
        var options = new CKEditorOptions
        {
            DefaultLicenseKey = jwt,
            Presets = new Dictionary<string, PresetConfig>
            {
                ["my_preset"] = new PresetConfig(),
                ["another_preset"] = new PresetConfig()
            }
        };

        var manager = new ConfigManager(Options.Create(options));

        Assert.Equal(jwt, manager.GetPresets()["default"].LicenseKey.Raw);
        Assert.Equal(jwt, manager.GetPresets()["my_preset"].LicenseKey.Raw);
    }

    [Fact]
    public void Constructor_RegistersPresetsFromOptions()
    {
        var options = new CKEditorOptions
        {
            Presets = new Dictionary<string, PresetConfig>
            {
                ["my_preset"] = new PresetConfig()
            }
        };

        var manager = new ConfigManager(Options.Create(options));

        Assert.True(manager.GetPresets().ContainsKey("my_preset"));
        Assert.True(manager.GetPresets().ContainsKey("default"));
    }

    [Fact]
    public void Constructor_RegistersContextsFromOptions()
    {
        var options = new CKEditorOptions
        {
            Contexts = new Dictionary<string, ContextConfig>
            {
                ["my_context"] = new ContextConfig()
            }
        };

        var manager = new ConfigManager(Options.Create(options));

        Assert.True(manager.GetContexts().ContainsKey("my_context"));
        Assert.True(manager.GetContexts().ContainsKey("default"));
    }

    [Fact]
    public void CreateDefaultContext_ReturnsEmptyConfig()
    {
        var context = ConfigManager.CreateDefaultContext();

        Assert.NotNull(context);
        Assert.NotNull(context.Config);
        Assert.Empty(context.Config);
    }

    [Fact]
    public void CreateDefaultPreset_SetsDefaultPluginsAndToolbar()
    {
        var preset = ConfigManager.CreateDefaultPreset();

        Assert.NotNull(preset);
        Assert.Equal(EditorType.Classic, preset.EditorType);
        Assert.True(preset.LicenseKey.IsGPL());

        Assert.NotNull(preset.Config);
        Assert.True(preset.Config.ContainsKey("toolbar"));
        Assert.True(preset.Config.ContainsKey("plugins"));
    }

    [Fact]
    public void RegisterPreset_AddsToPresetsDictionary()
    {
        var manager = new ConfigManager(CreateOptions());
        var newPreset = new PresetConfig();

        manager.RegisterPreset("custom_preset", newPreset);

        Assert.Same(newPreset, manager.GetPresets()["custom_preset"]);
    }

    [Fact]
    public void RegisterContext_AddsToContextsDictionary()
    {
        var manager = new ConfigManager(CreateOptions());
        var newContext = new ContextConfig();

        manager.RegisterContext("custom_context", newContext);

        Assert.Same(newContext, manager.GetContexts()["custom_context"]);
    }

    [Theory]
    [InlineData(null)]
    public void ResolvePreset_WithNull_ReturnsDefaultPreset(object? presetArg)
    {
        var manager = new ConfigManager(CreateOptions());

        var result = manager.ResolvePreset(presetArg);

        Assert.Same(manager.GetPresets()["default"], result);
    }

    [Fact]
    public void ResolvePreset_WithString_ReturnsNamedPreset()
    {
        var manager = new ConfigManager(CreateOptions());
        var preset = new PresetConfig();
        manager.RegisterPreset("my_preset", preset);

        var result = manager.ResolvePreset("my_preset");

        Assert.Same(preset, result);
    }

    [Fact]
    public void ResolvePreset_WithPresetObject_ReturnsSameObject()
    {
        var manager = new ConfigManager(CreateOptions());
        var preset = new PresetConfig();

        var result = manager.ResolvePreset(preset);

        Assert.Same(preset, result);
    }

    [Fact]
    public void ResolvePreset_WithUnknownName_ThrowsUnknownPresetException()
    {
        var manager = new ConfigManager(CreateOptions());

        var ex = Assert.Throws<UnknownPresetException>(() => manager.ResolvePreset("unknown"));
        Assert.Equal("Unknown preset: unknown", ex.Message);
    }

    [Fact]
    public void ResolvePreset_WithInvalidType_ThrowsArgumentException()
    {
        var manager = new ConfigManager(CreateOptions());

        var ex = Assert.Throws<ArgumentException>(() => manager.ResolvePreset(123));
        Assert.Equal("Invalid preset type (Parameter 'preset')", ex.Message);
        Assert.Equal("preset", ex.ParamName);
    }

    [Fact]
    public void ResolvePreset_WithMissingDefault_ReturnsNewPresetConfig()
    {
        var manager = new ConfigManager(CreateOptions());
        manager.RegisterPreset("default", null!);

        var result = manager.ResolvePreset(null);

        Assert.NotNull(result);
        Assert.IsType<PresetConfig>(result);
    }

    [Fact]
    public void ResolvePresetOrThrow_WithKnownName_ReturnsPreset()
    {
        var manager = new ConfigManager(CreateOptions());

        var result = manager.ResolvePresetOrThrow("default");

        Assert.Same(manager.GetPresets()["default"], result);
    }

    [Fact]
    public void ResolvePresetOrThrow_WithUnknownName_ThrowsUnknownPresetException()
    {
        var manager = new ConfigManager(CreateOptions());

        Assert.Throws<UnknownPresetException>(() => manager.ResolvePresetOrThrow("unknown"));
    }

    [Fact]
    public void ResolveContext_WithNull_ReturnsDefaultContext()
    {
        var manager = new ConfigManager(CreateOptions());

        var result = manager.ResolveContext(null);

        Assert.Same(manager.GetContexts()["default"], result);
    }

    [Fact]
    public void ResolveContext_WithString_ReturnsNamedContext()
    {
        var manager = new ConfigManager(CreateOptions());
        var context = new ContextConfig();
        manager.RegisterContext("my_context", context);

        var result = manager.ResolveContext("my_context");

        Assert.Same(context, result);
    }

    [Fact]
    public void ResolveContext_WithContextObject_ReturnsSameObject()
    {
        var manager = new ConfigManager(CreateOptions());
        var context = new ContextConfig();

        var result = manager.ResolveContext(context);

        Assert.Same(context, result);
    }

    [Fact]
    public void ResolveContext_WithUnknownName_ThrowsUnknownContextException()
    {
        var manager = new ConfigManager(CreateOptions());

        Assert.Throws<UnknownContextException>(() => manager.ResolveContext("unknown"));
    }

    [Fact]
    public void ResolveContext_WithInvalidType_ThrowsArgumentException()
    {
        var manager = new ConfigManager(CreateOptions());

        var ex = Assert.Throws<ArgumentException>(() => manager.ResolveContext(123));
        Assert.Equal("Invalid context type (Parameter 'context')", ex.Message);
        Assert.Equal("context", ex.ParamName);
    }

    [Fact]
    public void ResolveContext_WithMissingDefault_ReturnsNewContextConfig()
    {
        var manager = new ConfigManager(CreateOptions());
        manager.RegisterContext("default", null!);

        var result = manager.ResolveContext(null);

        Assert.NotNull(result);
        Assert.IsType<ContextConfig>(result);
    }

    [Fact]
    public void ResolveContextOrThrow_WithKnownName_ReturnsContext()
    {
        var manager = new ConfigManager(CreateOptions());

        var result = manager.ResolveContextOrThrow("default");

        Assert.Same(manager.GetContexts()["default"], result);
    }

    [Fact]
    public void ResolveContextOrThrow_WithUnknownName_ThrowsUnknownContextException()
    {
        var manager = new ConfigManager(CreateOptions());

        Assert.Throws<UnknownContextException>(() => manager.ResolveContextOrThrow("unknown"));
    }

    [Fact]
    public void Constructor_DoesNotOverridePresetLicense_WhenDefaultLicenseIsGPL()
    {
        var preset = new PresetConfig();
        var options = new CKEditorOptions
        {
            DefaultLicenseKey = "GPL",
            Presets = new Dictionary<string, PresetConfig>
            {
                ["my_preset"] = preset
            }
        };

        var manager = new ConfigManager(Options.Create(options));

        Assert.Same(preset, manager.GetPresets()["my_preset"]);
    }

    [Fact]
    public void Constructor_DoesNotOverridePresetLicense_WhenPresetHasNonGplLicense()
    {
        var jwt = LicenseKeyParser.Parse(JwtTestHelper.BuildValid("sh"));
        var preset = new PresetConfig().WithLicenseKey(jwt);

        var options = new CKEditorOptions
        {
            DefaultLicenseKey = JwtTestHelper.BuildValid("cloud"),
            Presets = new Dictionary<string, PresetConfig>
            {
                ["my_preset"] = preset
            }
        };

        var manager = new ConfigManager(Options.Create(options));

        Assert.Same(preset, manager.GetPresets()["my_preset"]);
        Assert.Equal(jwt, manager.GetPresets()["my_preset"].LicenseKey);
    }
}
