using CKEditor.Blazor.Components.Assets;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.SelfHosted;
using CKEditor.Blazor.Services;

namespace CKEditor.Blazor.Tests.Components.Assets;

public class CKE5SelfHostedTests : BunitContext
{
    private const string _basePath = "/_content/CKEditor.Blazor";
    private const string _editorVersion = "44.0.0";

    private static PresetConfig BuildPreset(string version = _editorVersion) => new()
    {
        SelfHosted = new SelfHostedConfig { EditorVersion = version, AssetsBasePath = _basePath }
    };

    public CKE5SelfHostedTests() => Services.AddCKEditor(options => options.Presets["default"] = BuildPreset());

    [Fact]
    public void RendersModulePreloadLink_ForEditorScript()
    {
        var cut = Render<CKE5SelfHosted>();

        var links = cut.FindAll("link[rel='modulepreload']").Select(l => l.GetAttribute("href")).ToList();
        Assert.Contains($"{_basePath}/ckeditor5/{_editorVersion}/dist/browser/ckeditor5.js", links);
    }

    [Fact]
    public void RendersCssLink_ForEditorStylesheet()
    {
        var cut = Render<CKE5SelfHosted>();

        var link = cut.Find("link[rel='stylesheet']");
        Assert.Equal($"{_basePath}/ckeditor5/{_editorVersion}/dist/browser/ckeditor5.css", link.GetAttribute("href"));
    }

    [Fact]
    public void RendersImportMap_ContainingEditorAndBlazorEntries()
    {
        var cut = Render<CKE5SelfHosted>();

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("ckeditor5", importMapScript.TextContent);
        Assert.Contains("ckeditor5-blazor", importMapScript.TextContent);
    }

    [Fact]
    public void DoesNotRenderImportMap_WhenEmitImportMapIsFalse()
    {
        var cut = Render<CKE5SelfHosted>(p => p.Add(p => p.EmitImportMap, false));

        Assert.Empty(cut.FindAll("script[type='importmap']"));
    }

    [Fact]
    public void RendersNonce_OnAllElements_WhenNonceProvided()
    {
        var cut = Render<CKE5SelfHosted>(p => p.Add(p => p.Nonce, "test-nonce"));

        foreach (var element in cut.FindAll("[nonce]"))
        {
            Assert.Equal("test-nonce", element.GetAttribute("nonce"));
        }
    }

    [Fact]
    public void RendersCustomImportMap_MergedIntoImportMap()
    {
        var cut = Render<CKE5SelfHosted>(p => p
            .Add(p => p.CustomImportMap, new Dictionary<string, string>
            {
                ["my-package"] = "/_content/CKEditor.Blazor/my-package.mjs"
            }));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("my-package", importMapScript.TextContent);
        Assert.Contains("/_content/CKEditor.Blazor/my-package.mjs", importMapScript.TextContent);
    }

    [Fact]
    public void UsesCustomPreset_WhenPresetParameterProvided()
    {
        Services.AddCKEditor(options =>
        {
            options.Presets["default"] = BuildPreset();
            options.Presets["v2"] = BuildPreset(version: "99.0.0");
        });

        var cut = Render<CKE5SelfHosted>(p => p.Add(p => p.Preset, "v2"));

        var links = cut.FindAll("link[rel='modulepreload']").Select(l => l.GetAttribute("href")).ToList();
        Assert.Contains($"{_basePath}/ckeditor5/99.0.0/dist/browser/ckeditor5.js", links);
    }

    [Fact]
    public void DoesNotRenderModulePreloadLinks_WhenEmitModulePreloadIsFalse()
    {
        var cut = Render<CKE5SelfHosted>(p => p.Add(p => p.EmitModulePreload, false));

        Assert.Empty(cut.FindAll("link[rel='modulepreload']"));
    }

    [Fact]
    public void StillRendersCssAndImportMap_WhenEmitModulePreloadIsFalse()
    {
        var cut = Render<CKE5SelfHosted>(p => p.Add(p => p.EmitModulePreload, false));

        Assert.NotNull(cut.Find("link[rel='stylesheet']"));
        Assert.NotNull(cut.Find("script[type='importmap']"));
    }
}
