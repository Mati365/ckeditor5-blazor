using CKEditor.Blazor.Exceptions;
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
            throw new ConfigurationException("Self-hosted config requires 'EditorVersion'.");
        }

        return BuildBundles(selfHosted)
            .Aggregate((a, b) => a.Merge(b))
            .WithMergedJs([AssetsBundle.BlazorIntegrationAsset]);
    }

    private IEnumerable<AssetsBundle> BuildBundles(SelfHostedConfig selfHosted)
    {
        yield return editorBuilder.Build(selfHosted.EditorVersion, selfHosted.AssetsBasePath);

        if (selfHosted.Premium)
        {
            yield return premiumBuilder.Build(selfHosted.EditorVersion, selfHosted.AssetsBasePath);
        }

        if (selfHosted.CKBox is { } ckbox)
        {
            yield return ckboxBuilder.Build(
                ckbox.Version ?? throw new ConfigurationException("Self-hosted config requires CKBox 'Version' when CKBox is enabled."),
                ckbox.Translations,
                selfHosted.AssetsBasePath,
                ckbox.Theme ?? "lark");
        }
    }
}
