using CKEditor.Blazor.Components.Assets;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.Cloud;
using CKEditor.Blazor.Services;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CKEditor.Blazor.Tests.Components.Assets;

public class CKE5CloudImportmapTests : BunitContext
{
    private const string _cdnUrl = "https://custom.cdn.example.com";
    private const string _editorVersion = "44.0.0";

    private static PresetConfig BuildPreset(string cdnUrl = _cdnUrl, string version = _editorVersion) => new()
    {
        Cloud = new CloudConfig { CdnUrl = cdnUrl, EditorVersion = version }
    };

    public CKE5CloudImportmapTests() => Services.AddCKEditor(options => options.Presets["default"] = BuildPreset());

    [Fact]
    public void RendersImportMap_ContainingEditorAndBlazorEntries()
    {
        var cut = Render<CKE5CloudImportmap>();

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("ckeditor5", importMapScript.TextContent);
        Assert.Contains("ckeditor5-blazor", importMapScript.TextContent);
    }

    [Fact]
    public void DoesNotRenderModulePreloadLinks()
    {
        var cut = Render<CKE5CloudImportmap>();

        Assert.Empty(cut.FindAll("link[rel='modulepreload']"));
    }

    [Fact]
    public void DoesNotRenderStylesheets()
    {
        var cut = Render<CKE5CloudImportmap>();

        Assert.Empty(cut.FindAll("link[rel='stylesheet']"));
    }

    [Fact]
    public void DoesNotRenderScriptTags()
    {
        var cut = Render<CKE5CloudImportmap>();

        // Should only contain the importmap script
        var scripts = cut.FindAll("script");
        Assert.All(scripts, s => Assert.Equal("importmap", s.GetAttribute("type")));
    }

    [Fact]
    public void RendersNonce_OnImportMapScript_WhenNonceProvided()
    {
        var cut = Render<CKE5CloudImportmap>(p => p.Add(p => p.Nonce, "test-nonce"));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Equal("test-nonce", importMapScript.GetAttribute("nonce"));
    }

    [Fact]
    public void RendersCustomImportMap_MergedIntoImportMap()
    {
        var cut = Render<CKE5CloudImportmap>(p => p
            .Add(p => p.CustomImportMap, new Dictionary<string, string>
            {
                ["my-package"] = "https://cdn.example.com/my-package.mjs"
            }));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("my-package", importMapScript.TextContent);
        Assert.Contains("https://cdn.example.com/my-package.mjs", importMapScript.TextContent);
    }

    [Fact]
    public void UsesCustomPreset_WhenPresetParameterProvided()
    {
        Services.AddCKEditor(options =>
        {
            options.Presets["default"] = BuildPreset();
            options.Presets["v2"] = BuildPreset(version: "99.0.0");
        });

        var cut = Render<CKE5CloudImportmap>(p => p.Add(p => p.Preset, "v2"));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains($"{_cdnUrl}/ckeditor5/99.0.0/ckeditor5.js", importMapScript.TextContent);
    }

    [Fact]
    public void RendersNoImportMapScript_WhenBundleBuilderReturnsNull()
    {
        var mockBuilder = new Mock<ICloudBundleBuilder>();
        mockBuilder.Setup(b => b.Build(It.IsAny<CloudConfig>())).Returns((AssetsBundle)null!);
        Services.AddSingleton(mockBuilder.Object);

        var cut = Render<CKE5CloudImportmap>();

        Assert.Empty(cut.FindAll("script[type='importmap']"));
    }
}
