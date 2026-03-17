using CKEditor.Blazor.Components.Assets;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Cloud;
using CKEditor.Blazor.Model.License;
using CKEditor.Blazor.Model.SelfHosted;
using CKEditor.Blazor.Services;

namespace CKEditor.Blazor.Tests.Components.Assets;

public class CKE5AssetsTests : BunitContext
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
    public void Cloud_RendersModulePreloadLink_ForEditorScript()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Assets>(static p => p.Add(static p => p.Distribution, DistributionChannel.Cloud));

        var links = cut.FindAll("link[rel='modulepreload']").Select(static l => l.GetAttribute("href")).ToList();
        Assert.Contains($"{_cdnUrl}/ckeditor5/{_cloudVersion}/ckeditor5.js", links);
    }

    [Fact]
    public void Cloud_RendersCssLink_ForEditorStylesheet()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Assets>(static p => p.Add(static p => p.Distribution, DistributionChannel.Cloud));

        var link = cut.Find("link[rel='stylesheet']");
        Assert.Equal($"{_cdnUrl}/ckeditor5/{_cloudVersion}/ckeditor5.css", link.GetAttribute("href"));
    }

    [Fact]
    public void Cloud_RendersImportMap_ContainingEditorAndBlazorEntries()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Assets>(static p => p.Add(static p => p.Distribution, DistributionChannel.Cloud));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("ckeditor5", importMapScript.TextContent);
        Assert.Contains("ckeditor5-blazor", importMapScript.TextContent);
    }

    [Fact]
    public void Cloud_DoesNotRenderImportMap_WhenEmitImportMapIsFalse()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Assets>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.Cloud)
            .Add(static p => p.EmitImportMap, false));

        Assert.Empty(cut.FindAll("script[type='importmap']"));
    }

    [Fact]
    public void Cloud_RendersNonce_OnAllElements_WhenNonceProvided()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Assets>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.Cloud)
            .Add(static p => p.Nonce, "test-nonce"));

        foreach (var element in cut.FindAll("[nonce]"))
        {
            Assert.Equal("test-nonce", element.GetAttribute("nonce"));
        }
    }

    [Fact]
    public void Cloud_RendersCustomImportMap_MergedIntoImportMap()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Assets>(static p => p
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

        var cut = Render<CKE5Assets>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.Cloud)
            .Add(static p => p.Preset, "v2"));

        var links = cut.FindAll("link[rel='modulepreload']").Select(static l => l.GetAttribute("href")).ToList();
        Assert.Contains($"{_cdnUrl}/ckeditor5/99.0.0/ckeditor5.js", links);
    }

    [Fact]
    public void Cloud_DoesNotRenderModulePreloadLinks_WhenEmitModulePreloadIsFalse()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Assets>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.Cloud)
            .Add(static p => p.EmitModulePreload, false));

        Assert.Empty(cut.FindAll("link[rel='modulepreload']"));
    }

    [Fact]
    public void Cloud_StillRendersCssAndImportMap_WhenEmitModulePreloadIsFalse()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildCloudPreset());

        var cut = Render<CKE5Assets>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.Cloud)
            .Add(static p => p.EmitModulePreload, false));

        Assert.NotNull(cut.Find("link[rel='stylesheet']"));
        Assert.NotNull(cut.Find("script[type='importmap']"));
    }

    [Fact]
    public void SH_RendersModulePreloadLink_ForEditorScript()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Assets>(static p => p.Add(static p => p.Distribution, DistributionChannel.SH));

        var links = cut.FindAll("link[rel='modulepreload']").Select(static l => l.GetAttribute("href")).ToList();
        Assert.Contains($"{_basePath}/ckeditor5/{_shVersion}/dist/browser/ckeditor5.js", links);
    }

    [Fact]
    public void DefaultsToSH_WhenDistributionIsNotProvided()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Assets>();

        var links = cut.FindAll("link[rel='modulepreload']").Select(static l => l.GetAttribute("href")).ToList();
        Assert.Contains($"{_basePath}/ckeditor5/{_shVersion}/dist/browser/ckeditor5.js", links);
    }

    [Fact]
    public void SH_RendersCssLink_ForEditorStylesheet()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Assets>(static p => p.Add(static p => p.Distribution, DistributionChannel.SH));

        var link = cut.Find("link[rel='stylesheet']");
        Assert.Equal($"{_basePath}/ckeditor5/{_shVersion}/dist/browser/ckeditor5.css", link.GetAttribute("href"));
    }

    [Fact]
    public void SH_RendersImportMap_ContainingEditorAndBlazorEntries()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Assets>(static p => p.Add(static p => p.Distribution, DistributionChannel.SH));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("ckeditor5", importMapScript.TextContent);
        Assert.Contains("ckeditor5-blazor", importMapScript.TextContent);
    }

    [Fact]
    public void SH_DoesNotRenderImportMap_WhenEmitImportMapIsFalse()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Assets>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.SH)
            .Add(static p => p.EmitImportMap, false));

        Assert.Empty(cut.FindAll("script[type='importmap']"));
    }

    [Fact]
    public void SH_RendersNonce_OnAllElements_WhenNonceProvided()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Assets>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.SH)
            .Add(static p => p.Nonce, "test-nonce"));

        foreach (var element in cut.FindAll("[nonce]"))
        {
            Assert.Equal("test-nonce", element.GetAttribute("nonce"));
        }
    }

    [Fact]
    public void SH_RendersCustomImportMap_MergedIntoImportMap()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Assets>(static p => p
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

        var cut = Render<CKE5Assets>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.SH)
            .Add(static p => p.Preset, "v2"));

        var links = cut.FindAll("link[rel='modulepreload']").Select(static l => l.GetAttribute("href")).ToList();
        Assert.Contains($"{_basePath}/ckeditor5/99.0.0/dist/browser/ckeditor5.js", links);
    }

    [Fact]
    public void SH_DoesNotRenderModulePreloadLinks_WhenEmitModulePreloadIsFalse()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Assets>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.SH)
            .Add(static p => p.EmitModulePreload, false));

        Assert.Empty(cut.FindAll("link[rel='modulepreload']"));
    }

    [Fact]
    public void SH_StillRendersCssAndImportMap_WhenEmitModulePreloadIsFalse()
    {
        Services.AddCKEditor(static options => options.Presets["default"] = BuildSHPreset());

        var cut = Render<CKE5Assets>(static p => p
            .Add(static p => p.Distribution, DistributionChannel.SH)
            .Add(static p => p.EmitModulePreload, false));

        Assert.NotNull(cut.Find("link[rel='stylesheet']"));
        Assert.NotNull(cut.Find("script[type='importmap']"));
    }

    [Fact]
    public void ThrowsInvalidOperationException_ForUnsupportedDistributionChannel()
    {
        Services.AddCKEditor(options => options.Presets["default"] = BuildSHPreset());

        var exception = Assert.Throws<InvalidOperationException>(() =>
            Render<CKE5Assets>(p => p.Add(x => x.Distribution, (DistributionChannel)999)));

        Assert.Contains("Unsupported distribution channel", exception.Message);
    }
}
