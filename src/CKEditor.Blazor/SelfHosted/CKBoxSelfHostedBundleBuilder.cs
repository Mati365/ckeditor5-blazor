using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.SelfHosted;

/// <summary>
/// Builds an asset bundle for self-hosted CKBox.
/// </summary>
public static class CKBoxSelfHostedBundleBuilder
{
    /// <summary>
    /// Builds an asset bundle for CKBox based on the provided version, translations, and theme.
    /// </summary>
    /// <param name="version">The CKBox version.</param>
    /// <param name="translations">List of translations.</param>
    /// <param name="basePath">The base path for assets.</param>
    /// <param name="theme">The theme name (defaults to 'lark').</param>
    /// <returns>The asset bundle.</returns>
    public static AssetsBundle Build(
        string version,
        IReadOnlyList<string> translations,
        string basePath,
        string theme = "lark")
    {
        var baseUrl = $"{basePath.TrimEnd('/')}/ckbox/{version.Trim('/')}/";

        var js = new List<JSAsset>
        {
            new()
            {
                Name = "ckbox",
                Url = $"{baseUrl}dist/ckbox.js",
                Type = JSAssetType.UMD
            }
        };

        // CKBox translations are UMD scripts, not ESM
        foreach (var translation in translations)
        {
            js.Add(new JSAsset
            {
                Name = $"ckbox/translations/{translation}",
                Url = $"{baseUrl}dist/translations/{translation}.js",
                Type = JSAssetType.UMD
            });
        }

        var css = new List<string> { $"{baseUrl}dist/styles/{theme}.css" };

        return new AssetsBundle(js, css);
    }
}
