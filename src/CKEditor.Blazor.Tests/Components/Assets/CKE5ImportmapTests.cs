using CKEditor.Blazor.Components.Assets;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.Cloud;
using CKEditor.Blazor.Model.License;
using CKEditor.Blazor.Model.SelfHosted;
using CKEditor.Blazor.Services;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;
using CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CKEditor.Blazor.Tests.Components.Assets;

public class CKE5ImportmapTests : BunitContext
{
    private const string _cdnUrl = "https://custom.cdn.example.com";
    private const string _cloudVersion = "44.0.0";
    private const string _basePath = "/_content/CKEditor.Blazor";
    private const string _shVersion = "44.0.0";

    private static PresetConfig BuildCloudPreset(string cdnUrl = _cdnUrl, string version = _cloudVersion) => new()
    {
        Cloud = new CloudConfig { CdnUrl = cdnUrl, EditorVersion = version }
    };

    private static PresetConfig BuildSHPreset(string version = _shVersion) => new()
    {
        SelfHosted = new SelfHostedConfig { EditorVersion = version, AssetsBasePath = _basePath }
    };

    [Fact]
    public void Cloud_RendersImportMap_ContainingEditorAndBlazorEntries()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Importmap>(static p => p.Add(static p => p.Distribution, DistributionChannel.Cloud));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("ckeditor5", importMapScript.TextContent);
        Assert.Contains("ckeditor5-blazor", importMapScript.TextContent);
    }

    [Fact]
    public void Cloud_DoesNotRenderModulePreloadLinks()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Importmap>(static p => p.Add(static p => p.Distribution, DistributionChannel.Cloud));

        Assert.Empty(cut.FindAll("link[rel='modulepreload']"));
    }

    [Fact]
    public void Cloud_DoesNotRenderStylesheets()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Importmap>(static p => p.Add(static p => p.Distribution, DistributionChannel.Cloud));

        Assert.Empty(cut.FindAll("link[rel='stylesheet']"));
    }

    [Fact]
    public void Cloud_DoesNotRenderScriptTags()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Importmap>(static p => p.Add(static p => p.Distribution, DistributionChannel.Cloud));

        // Should only contain the importmap script
        var scripts = cut.FindAll("script");
        Assert.All(scripts, static s => Assert.Equal("importmap", s.GetAttribute("type")));
    }

    [Fact]
    public void Cloud_RendersNonce_OnImportMapScript_WhenNonceProvided()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Importmap>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.Cloud)
            .Add(static p => p.Nonce, "test-nonce"));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Equal("test-nonce", importMapScript.GetAttribute("nonce"));
    }

    [Fact]
    public void Cloud_RendersCustomImportMap_MergedIntoImportMap()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Importmap>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.Cloud)
            .Add(static p => p.CustomImportMap, new Dictionary<string, string>
            {
                ["my-package"] = "https://cdn.example.com/my-package.mjs"
            }));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("my-package", importMapScript.TextContent);
        Assert.Contains("https://cdn.example.com/my-package.mjs", importMapScript.TextContent);
    }

    [Fact]
    public void Cloud_UsesCustomPreset_WhenPresetParameterProvided()
    {
        Services.AddCKEditor(static options =>
        {
            options.Presets["default"] = BuildCloudPreset();
            options.Presets["v2"] = BuildCloudPreset(version: "99.0.0");
        });

        var cut = Render<CKE5Importmap>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.Cloud)
            .Add(static p => p.Preset, "v2"));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains($"{_cdnUrl}/ckeditor5/99.0.0/ckeditor5.js", importMapScript.TextContent);
    }

    [Fact]
    public void Cloud_RendersNoImportMapScript_WhenBundleBuilderReturnsNull()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());
        var mockBuilder = new Mock<ICloudBundleBuilder>();
        mockBuilder.Setup(static b => b.Build(It.IsAny<CloudConfig>())).Returns((AssetsBundle)null!);
        Services.AddSingleton(mockBuilder.Object);

        var cut = Render<CKE5Importmap>(static p => p.Add(static p => p.Distribution, DistributionChannel.Cloud));

        Assert.Empty(cut.FindAll("script[type='importmap']"));
    }

    [Fact]
    public void SH_RendersImportMap_ContainingEditorAndBlazorEntries()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Importmap>(static p => p.Add(static p => p.Distribution, DistributionChannel.SH));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("ckeditor5", importMapScript.TextContent);
        Assert.Contains("ckeditor5-blazor", importMapScript.TextContent);
    }

    [Fact]
    public void DefaultsToSH_WhenDistributionIsNotProvided()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Importmap>();

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains($"{_basePath}/ckeditor5/{_shVersion}/dist/browser/ckeditor5.js", importMapScript.TextContent);
    }

    [Fact]
    public void SH_DoesNotRenderModulePreloadLinks()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Importmap>(static p => p.Add(static p => p.Distribution, DistributionChannel.SH));

        Assert.Empty(cut.FindAll("link[rel='modulepreload']"));
    }

    [Fact]
    public void SH_DoesNotRenderStylesheets()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Importmap>(static p => p.Add(static p => p.Distribution, DistributionChannel.SH));

        Assert.Empty(cut.FindAll("link[rel='stylesheet']"));
    }

    [Fact]
    public void SH_DoesNotRenderScriptTags()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Importmap>(static p => p.Add(static p => p.Distribution, DistributionChannel.SH));

        // Should only contain the importmap script
        var scripts = cut.FindAll("script");
        Assert.All(scripts, static s => Assert.Equal("importmap", s.GetAttribute("type")));
    }

    [Fact]
    public void SH_RendersNonce_OnImportMapScript_WhenNonceProvided()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Importmap>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.SH)
            .Add(static p => p.Nonce, "test-nonce"));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Equal("test-nonce", importMapScript.GetAttribute("nonce"));
    }

    [Fact]
    public void SH_RendersCustomImportMap_MergedIntoImportMap()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Importmap>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.SH)
            .Add(static p => p.CustomImportMap, new Dictionary<string, string>
            {
                ["my-package"] = "/_content/CKEditor.Blazor/my-package.mjs"
            }));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("my-package", importMapScript.TextContent);
        Assert.Contains("/_content/CKEditor.Blazor/my-package.mjs", importMapScript.TextContent);
    }

    [Fact]
    public void SH_UsesCustomPreset_WhenPresetParameterProvided()
    {
        Services.AddCKEditor(static options =>
        {
            options.Presets["default"] = BuildSHPreset();
            options.Presets["v2"] = BuildSHPreset(version: "99.0.0");
        });

        var cut = Render<CKE5Importmap>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.SH)
            .Add(static p => p.Preset, "v2"));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains($"{_basePath}/ckeditor5/99.0.0/dist/browser/ckeditor5.js", importMapScript.TextContent);
    }

    [Fact]
    public void SH_RendersNoImportMapScript_WhenBundleBuilderReturnsNull()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());
        var mockBuilder = new Mock<ISelfHostedBundleBuilder>();
        mockBuilder.Setup(static b => b.Build(It.IsAny<SelfHostedConfig>())).Returns((AssetsBundle)null!);
        Services.AddSingleton(mockBuilder.Object);

        var cut = Render<CKE5Importmap>(static p => p.Add(static p => p.Distribution, DistributionChannel.SH));

        Assert.Empty(cut.FindAll("script[type='importmap']"));
    }

    [Fact]
    public void ThrowsInvalidOperationException_ForUnsupportedDistributionChannel()
    {
        Services.AddCKEditor(options => options.Presets["default"] = BuildSHPreset());

        var exception = Assert.Throws<InvalidOperationException>(() =>
            Render<CKE5Importmap>(p => p.Add(x => x.Distribution, (DistributionChannel)999)));

        Assert.Contains("Unsupported distribution channel", exception.Message);
    }
}
