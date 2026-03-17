using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Tests.Model.Bundle;

public class AssetsBundleTests
{
    [Fact]
    public void AssetsBundle_ShouldInitializeCorrectly()
    {
        var jsList = new List<JSAsset> { new() { Name = "test", Url = "test.js", Type = JSAssetType.ESM } };
        var cssList = new List<string> { "style.css" };

        var bundle = new AssetsBundle(jsList, cssList);

        Assert.Equal(jsList, bundle.Js);
        Assert.Equal(cssList, bundle.Css);
    }

    [Fact]
    public void Merge_ShouldCombineBundles()
    {
        var bundle1 = new AssetsBundle(
            [new JSAsset { Name = "a", Url = "a.js", Type = JSAssetType.ESM }],
            ["a.css"]);
        var bundle2 = new AssetsBundle(
            [new JSAsset { Name = "b", Url = "b.js", Type = JSAssetType.UMD }],
            ["b.css"]);

        var result = bundle1.Merge(bundle2);

        Assert.Equal(2, result.Js.Count);
        Assert.Equal(2, result.Css.Count);
        Assert.Contains(result.Js, static x => x.Name == "a");
        Assert.Contains(result.Js, static x => x.Name == "b");
        Assert.Contains("a.css", result.Css);
        Assert.Contains("b.css", result.Css);
    }

    [Fact]
    public void GetEsmOnlyUrls_ShouldReturnOnlyEsmAndDistinct()
    {
        var bundle = new AssetsBundle(
            [
                new JSAsset { Name = "a", Url = "a.js", Type = JSAssetType.ESM },
                new JSAsset { Name = "b", Url = "b.js", Type = JSAssetType.UMD },
                new JSAsset { Name = "c", Url = "a.js", Type = JSAssetType.ESM } // duplicate url
            ],
            []);

        var result = bundle.GetEsmOnlyUrls();

        Assert.Single(result);
        Assert.Equal("a.js", result[0]);
    }

    [Fact]
    public void GetUmdUrls_ShouldReturnOnlyUmdAndDistinct()
    {
        var bundle = new AssetsBundle(
            [
                new JSAsset { Name = "a", Url = "a.js", Type = JSAssetType.UMD },
                new JSAsset { Name = "b", Url = "b.js", Type = JSAssetType.ESM },
                new JSAsset { Name = "c", Url = "a.js", Type = JSAssetType.UMD } // duplicate url
            ],
            []);

        var result = bundle.GetUmdUrls();

        Assert.Single(result);
        Assert.Equal("a.js", result[0]);
    }

    [Fact]
    public void GetCssUrls_ShouldReturnDistinctCss()
    {
        var bundle = new AssetsBundle(
            [],
            ["a.css", "b.css", "a.css"]);

        var result = bundle.GetCssUrls();

        Assert.Equal(2, result.Count);
        Assert.Contains("a.css", result);
        Assert.Contains("b.css", result);
    }

    [Fact]
    public void GetImportMap_ShouldReturnDictionaryForEsmAssets()
    {
        var bundle = new AssetsBundle(
            [
                new JSAsset { Name = "moduleA", Url = "a.js", Type = JSAssetType.ESM },
                new JSAsset { Name = "moduleB", Url = "b.js", Type = JSAssetType.UMD },
                new JSAsset { Name = "moduleC", Url = "c/", Type = JSAssetType.ESM_DIRECTORY },
                new JSAsset { Name = "moduleA", Url = "a_newer.js", Type = JSAssetType.ESM } // Will test grouping and taking last
            ],
            []);

        var map = bundle.GetImportMap();

        Assert.Equal(2, map.Count);
        Assert.Equal("a_newer.js", map["moduleA"]);
        Assert.Equal("c/", map["moduleC"]);
        Assert.False(map.ContainsKey("moduleB"));
    }

    [Fact]
    public void WithMergedJs_ShouldMergeJsAssets()
    {
        var bundle = new AssetsBundle(
            [new JSAsset { Name = "a", Url = "a.js", Type = JSAssetType.ESM }],
            []);

        var newJs = new List<JSAsset> { new() { Name = "b", Url = "b.js", Type = JSAssetType.UMD } };
        var result = bundle.WithMergedJs(newJs);

        Assert.Equal(2, result.Js.Count);
        Assert.Single(bundle.Js);
    }

    [Fact]
    public void WithMergedCss_ShouldMergeCssAssets()
    {
        var bundle = new AssetsBundle(
            [],
            ["a.css"]);

        var newCss = new List<string> { "b.css" };
        var result = bundle.WithMergedCss(newCss);

        Assert.Equal(2, result.Css.Count);
        Assert.Single(bundle.Css);
    }
}
