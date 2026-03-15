using CKEditor.Blazor.Model;
using CKEditor.Blazor.Services;
using CKEditor.Blazor.Tests.Helpers;

namespace CKEditor.Blazor.Tests.Services;

public class CKEditorOptionsTests
{
    [Fact]
    public void GetParsedLicenseKey_WithNullDefault_ReturnsGplLicenseKey()
    {
        var options = new CKEditorOptions { DefaultLicenseKey = null };
        var result = options.GetParsedLicenseKey();

        Assert.NotNull(result);
        Assert.True(result.IsGPL());
        Assert.Equal("GPL", result.Raw);
    }

    [Fact]
    public void GetParsedLicenseKey_WithEmptyDefault_ThrowsArgumentException()
    {
        var options = new CKEditorOptions { DefaultLicenseKey = string.Empty };

        Assert.Throws<ArgumentException>(() => options.GetParsedLicenseKey());
    }

    [Fact]
    public void GetParsedLicenseKey_WithInvalidDefault_ThrowsArgumentException()
    {
        var options = new CKEditorOptions { DefaultLicenseKey = "invalid_key" };

        Assert.Throws<ArgumentException>(() => options.GetParsedLicenseKey());
    }

    [Fact]
    public void GetParsedLicenseKey_WithValidJwt_ReturnsParsedLicenseKey()
    {
        var jwt = JwtTestHelper.BuildValid("sh");
        var options = new CKEditorOptions { DefaultLicenseKey = jwt };

        var result = options.GetParsedLicenseKey();

        Assert.NotNull(result);
        Assert.Equal(jwt, result.Raw);
        Assert.True(result.IsSelfHostedOnly());
    }

    [Fact]
    public void Presets_IsInitializedToEmptyDictionary()
    {
        var options = new CKEditorOptions();

        Assert.NotNull(options.Presets);
        Assert.Empty(options.Presets);
    }

    [Fact]
    public void SetLicenseKey_SetsDefaultLicenseKeyAndReturnsThis()
    {
        var options = new CKEditorOptions();

        var result = options.SetLicenseKey("GPL");

        Assert.Same(options, result);
        Assert.Equal("GPL", options.DefaultLicenseKey);
    }

    [Fact]
    public void AddPreset_WithPresetObject_RegistersPresetAndReturnsThis()
    {
        var options = new CKEditorOptions();
        var preset = new PresetConfig().WithLanguage("pl");

        var result = options.AddPreset("minimal", preset);

        Assert.Same(options, result);
        Assert.Same(preset, options.Presets["minimal"]);
    }

    [Fact]
    public void AddPreset_WithConfigureFunc_BuildsPresetFromBlankAndRegisters()
    {
        var options = new CKEditorOptions();

        options.AddPreset("minimal", p => p.WithLanguage("pl").WithPlugins("Essentials", "Bold"));

        var preset = options.Presets["minimal"];
        Assert.Equal("pl", preset.Config["language"]);
        Assert.Equal(new[] { "Essentials", "Bold" }, preset.Config["plugins"]);
    }

    [Fact]
    public void AddDefaultPreset_RegistersPresetUnderDefaultKey()
    {
        var options = new CKEditorOptions();
        var preset = new PresetConfig().WithEditorType(EditorType.Balloon);

        options.AddDefaultPreset(preset);

        Assert.Same(preset, options.Presets["default"]);
    }

    [Fact]
    public void AddDefaultPreset_WithConfigureFunc_BuildsPresetFromBlankAndRegisters()
    {
        var options = new CKEditorOptions();

        options.AddDefaultPreset(p => p.WithEditorType(EditorType.Balloon));

        Assert.Equal(EditorType.Balloon, options.Presets["default"].EditorType);
    }

    [Fact]
    public void ExtendDefaultPreset_WithConfigureFunc_BuildsPresetFromDefaultsAndRegisters()
    {
        var options = new CKEditorOptions();

        options.ExtendDefaultPreset(p => p.WithEditorType(EditorType.Balloon));

        Assert.Equal(EditorType.Balloon, options.Presets["default"].EditorType);
        Assert.NotEmpty((object[])((Dictionary<string, object>)options.Presets["default"].Config["toolbar"])["items"]);
    }

    [Fact]
    public void AddContext_WithContextObject_RegistersContextAndReturnsThis()
    {
        var options = new CKEditorOptions();
        var context = new ContextConfig { Plugins = ["Essentials"] };

        var result = options.AddContext("minimal", context);

        Assert.Same(options, result);
        Assert.Same(context, options.Contexts["minimal"]);
    }

    [Fact]
    public void AddContext_WithConfigureFunc_BuildsContextFromBlankAndRegisters()
    {
        var options = new CKEditorOptions();

        options.AddContext("minimal", c => c with { Plugins = ["Essentials"] });

        var context = options.Contexts["minimal"];
        Assert.Equal(["Essentials"], context.Plugins);
    }

    [Fact]
    public void AddDefaultContext_RegistersContextUnderDefaultKey()
    {
        var options = new CKEditorOptions();
        var context = new ContextConfig { Plugins = ["Essentials"] };

        options.AddDefaultContext(context);

        Assert.Same(context, options.Contexts["default"]);
    }

    [Fact]
    public void AddDefaultContext_WithConfigureFunc_BuildsContextFromBlankAndRegisters()
    {
        var options = new CKEditorOptions();

        options.AddDefaultContext(c => c with { Plugins = ["Essentials"] });

        Assert.Equal(["Essentials"], options.Contexts["default"].Plugins);
    }

    [Fact]
    public void ExtendDefaultContext_WithConfigureFunc_BuildsContextFromDefaultsAndRegisters()
    {
        var options = new CKEditorOptions();

        options.ExtendDefaultContext(c => c with { Plugins = ["Essentials"] });

        Assert.Equal(["Essentials"], options.Contexts["default"].Plugins);
    }

    [Fact]
    public void FluentChaining_BuildsOptionsCorrectly()
    {
        var options = new CKEditorOptions()
            .SetLicenseKey("GPL")
            .AddPreset("minimal", p => p.WithPlugins("Essentials"))
            .AddDefaultPreset(new PresetConfig().WithEditorType(EditorType.Inline));

        Assert.Equal("GPL", options.DefaultLicenseKey);
        Assert.True(options.Presets.ContainsKey("minimal"));
        Assert.Equal(EditorType.Inline, options.Presets["default"].EditorType);
    }
}
