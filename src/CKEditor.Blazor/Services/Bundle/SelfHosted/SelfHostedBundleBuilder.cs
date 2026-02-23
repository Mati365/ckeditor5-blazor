using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.SelfHosted;
using CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;

namespace CKEditor.Blazor.Services.Bundle.SelfHosted;

/// <summary>
/// Builds an <see cref="AssetsBundle"/> from a self-hosted configuration.
/// </summary>
public class SelfHostedBundleBuilder(
    ICKEditorSelfHostedBundleBuilder editorBuilder,
    ICKEditorPremiumSelfHostedBundleBuilder premiumBuilder,
    ICKBoxSelfHostedBundleBuilder ckboxBuilder) : ISelfHostedBundleBuilder
{
    public AssetsBundle Build(SelfHostedConfig selfHosted)
    {
        if (string.IsNullOrWhiteSpace(selfHosted.EditorVersion))
        {
            throw new InvalidOperationException("Self-hosted config requires 'EditorVersion'.");
        }

        var editorBundle = editorBuilder.Build(
            selfHosted.EditorVersion,
            selfHosted.AssetsBasePath);

        if (selfHosted.Premium)
        {
            var premiumBundle = premiumBuilder.Build(
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

            var ckboxBundle = ckboxBuilder.Build(
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
