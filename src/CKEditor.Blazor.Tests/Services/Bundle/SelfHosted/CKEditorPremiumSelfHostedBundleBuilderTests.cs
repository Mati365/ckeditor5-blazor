using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Bundle.SelfHosted;

namespace CKEditor.Blazor.Tests.Services.Bundle.SelfHosted;

public class CKEditorPremiumSelfHostedBundleBuilderTests
{
    [Theory]
    [InlineData("35.0.0", "/_content/ckeditor5")]
    [InlineData("40.0.1", "my-base-path/")]
    public void Build_GeneratesCorrectPremiumUrls_BasedOnVersionAndBasePath(string version, string basePath)
    {
        var bundle = new CKEditorPremiumSelfHostedBundleBuilder().Build(version, basePath);
        var baseUrl = $"{basePath.TrimEnd('/')}/ckeditor5-premium-features/{version.Trim('/')}/";

        Assert.Contains(bundle.Js, a => a is { Name: "ckeditor5-premium-features", Type: JSAssetType.ESM } && a.Url == $"{baseUrl}dist/browser/ckeditor5-premium-features.js");
        Assert.Contains(bundle.Js, a => a is { Name: "ckeditor5-premium-features/translations/", Type: JSAssetType.ESM_DIRECTORY } && a.Url == $"{baseUrl}dist/translations/");
        Assert.Contains(bundle.Css, c => c == $"{baseUrl}dist/browser/ckeditor5-premium-features.css");
        Assert.Equal(2, bundle.Js.Count);
        Assert.Single(bundle.Css);
    }
}
