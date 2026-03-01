using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Bundle.Cloud;

namespace CKEditor.Blazor.Tests.Services.Bundle.Cloud;

public class CKEditorPremiumCloudBundleBuilderTests
{
    [Theory]
    [InlineData("35.0.0", "https://cdn.example.com")]
    [InlineData("40.0.1", "https://cdn.ckeditor.com")]
    public void Build_GeneratesCorrectPremiumUrls_BasedOnVersionAndCdn(string version, string cdnUrl)
    {
        var bundle = new CKEditorPremiumCloudBundleBuilder().Build(version, cdnUrl);
        var baseUrl = $"{cdnUrl}/ckeditor5-premium-features/{version}/";

        Assert.Contains(bundle.Js, a => a is { Name: "ckeditor5-premium-features", Type: JSAssetType.ESM } && a.Url == $"{baseUrl}ckeditor5-premium-features.js");
        Assert.Contains(bundle.Js, a => a is { Name: "ckeditor5-premium-features/translations/", Type: JSAssetType.ESM_DIRECTORY } && a.Url == $"{baseUrl}translations/");
        Assert.Contains(bundle.Css, c => c == $"{baseUrl}ckeditor5-premium-features.css");
        Assert.Equal(2, bundle.Js.Count);
        Assert.Single(bundle.Css);
    }
}
