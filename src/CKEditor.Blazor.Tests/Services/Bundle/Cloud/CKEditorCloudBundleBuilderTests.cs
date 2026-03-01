using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Bundle.Cloud;

namespace CKEditor.Blazor.Tests.Services.Bundle.Cloud;

public class CKEditorCloudBundleBuilderTests
{
    [Theory]
    [InlineData("35.0.0", "https://cdn.example.com")]
    [InlineData("40.0.1", "https://cdn.ckeditor.com")]
    public void Build_GeneratesCorrectUrls_BasedOnVersionAndCdn(string version, string cdnUrl)
    {
        var bundle = new CKEditorCloudBundleBuilder().Build(version, cdnUrl);
        var baseUrl = $"{cdnUrl}/ckeditor5/{version}/";

        Assert.Contains(bundle.Js, a => a is { Name: "ckeditor5", Type: JSAssetType.ESM } && a.Url == $"{baseUrl}ckeditor5.js");
        Assert.Contains(bundle.Js, a => a is { Name: "ckeditor5/translations/", Type: JSAssetType.ESM_DIRECTORY } && a.Url == $"{baseUrl}translations/");
        Assert.Contains(bundle.Css, c => c == $"{baseUrl}ckeditor5.css");
        Assert.Equal(2, bundle.Js.Count);
        Assert.Single(bundle.Css);
    }
}
