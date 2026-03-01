using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Bundle.SelfHosted;

namespace CKEditor.Blazor.Tests.Services.Bundle.SelfHosted;

public class CKEditorSelfHostedBundleBuilderTests
{
    [Theory]
    [InlineData("35.0.0", "/_content/ckeditor5")]
    [InlineData("40.0.1", "my-base-path/")]
    public void Build_GeneratesCorrectUrls_BasedOnVersionAndBasePath(string version, string basePath)
    {
        var bundle = new CKEditorSelfHostedBundleBuilder().Build(version, basePath);
        var baseUrl = $"{basePath.TrimEnd('/')}/ckeditor5/{version.Trim('/')}/";

        Assert.Contains(bundle.Js, a => a is { Name: "ckeditor5", Type: JSAssetType.ESM } && a.Url == $"{baseUrl}dist/browser/ckeditor5.js");
        Assert.Contains(bundle.Js, a => a is { Name: "ckeditor5/translations/", Type: JSAssetType.ESM_DIRECTORY } && a.Url == $"{baseUrl}dist/translations/");
        Assert.Contains(bundle.Css, c => c == $"{baseUrl}dist/browser/ckeditor5.css");
        Assert.Equal(2, bundle.Js.Count);
        Assert.Single(bundle.Css);
    }
}
