using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Bundle.Cloud;

namespace CKEditor.Blazor.Tests.Services.Bundle.Cloud;

public class CKBoxCloudBundleBuilderTests
{
    [Theory]
    [InlineData("2.0.0", "https://cdn.example.com", "lark", new string[] { })]
    [InlineData("2.1.1", "https://cdn.ckbox.com/", "other-theme", new[] { "en", "pl" })]
    public void Build_GeneratesCorrectCKBoxUrls(string version, string cdnUrl, string theme, string[] translations)
    {
        var builder = new CKBoxCloudBundleBuilder();
        var bundle = builder.Build(version, translations, cdnUrl, theme);

        var baseUrl = $"{cdnUrl.TrimEnd('/')}/ckbox/{version.Trim('/')}/";

        Assert.Contains(bundle.Js, a => a is { Name: "ckbox", Type: JSAssetType.UMD } && a.Url == $"{baseUrl}ckbox.js");
        Assert.Contains(bundle.Css, c => c == $"{baseUrl}styles/themes/{theme}.css");

        foreach (var t in translations)
        {
            Assert.Contains(bundle.Js, a => a is { Type: JSAssetType.UMD } && a.Name == $"ckbox/translations/{t}" && a.Url == $"{baseUrl}translations/{t}.js");
        }

        Assert.Equal(1 + translations.Length, bundle.Js.Count);
        Assert.Single(bundle.Css);
    }
}
