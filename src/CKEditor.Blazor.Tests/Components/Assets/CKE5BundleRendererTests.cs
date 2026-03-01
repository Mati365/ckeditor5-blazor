using CKEditor.Blazor.Components.Assets;
using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Tests.Components.Assets;

public class CKE5BundleRendererTests : BunitContext
{
    private static AssetsBundle CreateBundle(
        IReadOnlyList<JSAsset>? js = null,
        IReadOnlyList<string>? css = null) =>
        new(js ?? [], css ?? []);

    private static JSAsset EsmAsset(string name, string url) =>
        new() { Name = name, Url = url, Type = JSAssetType.ESM };

    private static JSAsset UmdAsset(string name, string url) =>
        new() { Name = name, Url = url, Type = JSAssetType.UMD };

    [Fact]
    public void RendersNothing_WhenBundleIsNull()
    {
        var cut = Render<CKE5BundleRenderer>();

        Assert.Empty(cut.Nodes);
    }

    [Fact]
    public void RendersModulePreloadLinks_ForEsmAssets()
    {
        var bundle = CreateBundle(js: [EsmAsset("ckeditor5", "https://cdn.example.com/ckeditor5.mjs")]);
        var cut = Render<CKE5BundleRenderer>(p => p.Add(p => p.Bundle, bundle));

        var link = cut.Find("link[rel='modulepreload']");
        Assert.Equal("https://cdn.example.com/ckeditor5.mjs", link.GetAttribute("href"));
        Assert.Equal("anonymous", link.GetAttribute("crossorigin"));
    }

    [Fact]
    public void RendersCssLinks_ForCssAssets()
    {
        var bundle = CreateBundle(css: ["https://cdn.example.com/styles.css"]);
        var cut = Render<CKE5BundleRenderer>(p => p.Add(p => p.Bundle, bundle));

        var link = cut.Find("link[rel='stylesheet']");
        Assert.Equal("https://cdn.example.com/styles.css", link.GetAttribute("href"));
        Assert.Equal("anonymous", link.GetAttribute("crossorigin"));
    }

    [Fact]
    public void RendersScriptTags_ForUmdAssets()
    {
        var bundle = CreateBundle(js: [UmdAsset("ckeditor5-umd", "https://cdn.example.com/ckeditor5.umd.js")]);
        var cut = Render<CKE5BundleRenderer>(p => p.Add(p => p.Bundle, bundle));

        var script = cut.Find("script[src]");
        Assert.Equal("https://cdn.example.com/ckeditor5.umd.js", script.GetAttribute("src"));
    }

    [Fact]
    public void RendersImportMap_WithEsmAssets()
    {
        var bundle = CreateBundle(js: [EsmAsset("ckeditor5", "https://cdn.example.com/ckeditor5.mjs")]);
        var cut = Render<CKE5BundleRenderer>(p => p.Add(p => p.Bundle, bundle));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.NotNull(importMapScript);
        Assert.Contains("ckeditor5", importMapScript.TextContent);
        Assert.Contains("https://cdn.example.com/ckeditor5.mjs", importMapScript.TextContent);
    }

    [Fact]
    public void DoesNotRenderImportMap_WhenEmitImportMapIsFalse()
    {
        var bundle = CreateBundle(js: [EsmAsset("ckeditor5", "https://cdn.example.com/ckeditor5.mjs")]);
        var cut = Render<CKE5BundleRenderer>(p => p
            .Add(p => p.Bundle, bundle)
            .Add(p => p.EmitImportMap, false));

        Assert.Empty(cut.FindAll("script[type='importmap']"));
    }

    [Fact]
    public void DoesNotRenderImportMap_WhenImportMapIsEmpty()
    {
        // UMD assets do NOT appear in the import map
        var bundle = CreateBundle(js: [UmdAsset("ckeditor5-umd", "https://cdn.example.com/ckeditor5.umd.js")]);
        var cut = Render<CKE5BundleRenderer>(p => p.Add(p => p.Bundle, bundle));

        Assert.Empty(cut.FindAll("script[type='importmap']"));
    }

    [Fact]
    public void RendersNonce_OnAllElements_WhenNonceProvided()
    {
        var bundle = CreateBundle(
            js: [EsmAsset("ckeditor5", "https://cdn.example.com/ckeditor5.mjs")],
            css: ["https://cdn.example.com/styles.css"]);

        var cut = Render<CKE5BundleRenderer>(p => p
            .Add(p => p.Bundle, bundle)
            .Add(p => p.Nonce, "test-nonce"));

        foreach (var element in cut.FindAll("[nonce]"))
        {
            Assert.Equal("test-nonce", element.GetAttribute("nonce"));
        }
    }

    [Fact]
    public void DoesNotRenderNonce_WhenNonceNotProvided()
    {
        var bundle = CreateBundle(js: [EsmAsset("ckeditor5", "https://cdn.example.com/ckeditor5.mjs")]);
        var cut = Render<CKE5BundleRenderer>(p => p.Add(p => p.Bundle, bundle));

        Assert.Empty(cut.FindAll("[nonce]"));
    }

    [Fact]
    public void RendersCustomImportMap_MergedIntoImportMap()
    {
        var bundle = CreateBundle(js: [EsmAsset("ckeditor5", "https://cdn.example.com/ckeditor5.mjs")]);
        var cut = Render<CKE5BundleRenderer>(p => p
            .Add(p => p.Bundle, bundle)
            .Add(p => p.CustomImportMap, new Dictionary<string, string>
            {
                ["my-custom-package"] = "https://cdn.example.com/custom.mjs"
            }));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("my-custom-package", importMapScript.TextContent);
        Assert.Contains("https://cdn.example.com/custom.mjs", importMapScript.TextContent);
    }

    [Fact]
    public void CustomImportMap_OverridesBundle_WhenKeyConflicts()
    {
        var bundle = CreateBundle(js: [EsmAsset("ckeditor5", "https://cdn.example.com/original.mjs")]);
        var cut = Render<CKE5BundleRenderer>(p => p
            .Add(p => p.Bundle, bundle)
            .Add(p => p.CustomImportMap, new Dictionary<string, string>
            {
                ["ckeditor5"] = "https://cdn.example.com/overridden.mjs"
            }));

        var importMapScript = cut.Find("script[type='importmap']");
        Assert.Contains("https://cdn.example.com/overridden.mjs", importMapScript.TextContent);
        Assert.DoesNotContain("https://cdn.example.com/original.mjs", importMapScript.TextContent);
    }

    [Fact]
    public void RendersMultipleEsmAssets_AsMultipleModulePreloadLinks()
    {
        var bundle = CreateBundle(js:
        [
            EsmAsset("ckeditor5", "https://cdn.example.com/ckeditor5.mjs"),
            EsmAsset("ckeditor5-premium", "https://cdn.example.com/premium.mjs")
        ]);
        var cut = Render<CKE5BundleRenderer>(p => p.Add(p => p.Bundle, bundle));

        Assert.Equal(2, cut.FindAll("link[rel='modulepreload']").Count);
    }

    [Fact]
    public void DeduplicatesEsmAssets_WhenSameUrlAppearsMultipleTimes()
    {
        var bundle = CreateBundle(js:
        [
            EsmAsset("ckeditor5", "https://cdn.example.com/ckeditor5.mjs"),
            EsmAsset("ckeditor5", "https://cdn.example.com/ckeditor5.mjs")
        ]);
        var cut = Render<CKE5BundleRenderer>(p => p.Add(p => p.Bundle, bundle));

        Assert.Single(cut.FindAll("link[rel='modulepreload']"));
    }

    [Fact]
    public void DoesNotRenderModulePreloadLinks_WhenEmitModulePreloadIsFalse()
    {
        var bundle = CreateBundle(js: [EsmAsset("ckeditor5", "https://cdn.example.com/ckeditor5.mjs")]);
        var cut = Render<CKE5BundleRenderer>(p => p
            .Add(p => p.Bundle, bundle)
            .Add(p => p.EmitModulePreload, false));

        Assert.Empty(cut.FindAll("link[rel='modulepreload']"));
    }

    [Fact]
    public void StillRendersImportMapAndCss_WhenEmitModulePreloadIsFalse()
    {
        var bundle = CreateBundle(
            js: [EsmAsset("ckeditor5", "https://cdn.example.com/ckeditor5.mjs")],
            css: ["https://cdn.example.com/styles.css"]);

        var cut = Render<CKE5BundleRenderer>(p => p
            .Add(p => p.Bundle, bundle)
            .Add(p => p.EmitModulePreload, false));

        Assert.Empty(cut.FindAll("link[rel='modulepreload']"));
        Assert.NotNull(cut.Find("script[type='importmap']"));
        Assert.NotNull(cut.Find("link[rel='stylesheet']"));
    }
}
