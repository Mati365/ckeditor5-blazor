using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.SelfHosted;

namespace CKEditor.Blazor.SelfHosted;

/// <summary>
/// Builds an <see cref="AssetsBundle"/> from a self-hosted configuration.
/// </summary>
public static class SelfHostedBundleBuilder
{
    /// <summary>
    /// Creates an <see cref="AssetsBundle"/> from the given self-hosted configuration.
    /// </summary>
    /// <param name="selfHosted">The self-hosted configuration.</param>
    /// <returns>The resulting assets bundle.</returns>
    public static AssetsBundle Build(SelfHostedConfig selfHosted)
    {
        if (string.IsNullOrWhiteSpace(selfHosted.EditorVersion))
        {
            throw new InvalidOperationException("Self-hosted config requires 'EditorVersion'.");
        }

        var editorBundle = CKEditorSelfHostedBundleBuilder.Build(
            selfHosted.EditorVersion,
            selfHosted.AssetsBasePath);

        if (selfHosted.Premium)
        {
            var premiumBundle = CKEditorPremiumSelfHostedBundleBuilder.Build(
                selfHosted.EditorVersion,
                selfHosted.AssetsBasePath);

            editorBundle = editorBundle.Merge(premiumBundle);
        }

        if (selfHosted.CKBox is not null)
        {
            if (string.IsNullOrWhiteSpace(selfHosted.CKBox.Version))
            {
                throw new InvalidOperationException("Self-hosted config requires CKBox 'Version' when CKBox is enabled.");
            }

            var ckboxBundle = CKBoxSelfHostedBundleBuilder.Build(
                selfHosted.CKBox.Version,
                selfHosted.CKBox.Translations,
                selfHosted.AssetsBasePath,
                selfHosted.CKBox.Theme ?? "lark");

            editorBundle = editorBundle.Merge(ckboxBundle);
        }

        editorBundle.Js.Add(new JSAsset
        {
            Name = "ckeditor5-blazor",
            Url = "/_content/CKEditor.Blazor/ckeditor5-blazor/index.mjs",
            Type = JSAssetType.ESM
        });

        return editorBundle;
    }
}
