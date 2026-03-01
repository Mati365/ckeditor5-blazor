using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Bundle.SelfHosted;

namespace CKEditor.Blazor.Tests.Services.Bundle.SelfHosted;

public class CKBoxSelfHostedBundleBuilderTests
{
    [Theory]
    [InlineData("2.0.0", "/_content/ckbox", "lark", new string[] { })]
    [InlineData("2.1.1", "my-base-path/", "other-theme", new[] { "en", "pl" })]
    public void Build_GeneratesCorrectCKBoxUrls(string version, string basePath, string theme, string[] translations)
    {
        var builder = new CKBoxSelfHostedBundleBuilder();
        var bundle = builder.Build(version, translations, basePath, theme);

        var baseUrl = $"{basePath.TrimEnd('/')}/ckbox/{version.Trim('/')}/";

        Assert.Contains(bundle.Js, a => a is { Name: "ckbox", Type: JSAssetType.UMD } && a.Url == $"{baseUrl}dist/ckbox.js");
        Assert.Contains(bundle.Css, c => c == $"{baseUrl}dist/styles/{theme}.css");

        foreach (var t in translations)
        {
            Assert.Contains(bundle.Js, a => a is { Type: JSAssetType.UMD } && a.Name == $"ckbox/translations/{t}" && a.Url == $"{baseUrl}dist/translations/{t}.js");
        }

        Assert.Equal(1 + translations.Length, bundle.Js.Count);
        Assert.Single(bundle.Css);
    }
}
