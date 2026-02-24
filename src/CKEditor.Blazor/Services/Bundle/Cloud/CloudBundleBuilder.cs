using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.Cloud;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;

namespace CKEditor.Blazor.Services.Bundle.Cloud;

/// <summary>
/// Builds an <see cref="AssetsBundle"/> from a cloud configuration.
/// </summary>
public class CloudBundleBuilder(
    ICKEditorCloudBundleBuilder editorBuilder,
    ICKEditorPremiumCloudBundleBuilder premiumBuilder,
    ICKBoxCloudBundleBuilder ckboxBuilder) : ICloudBundleBuilder
{
    public AssetsBundle Build(CloudConfig cloud)
    {
        if (string.IsNullOrWhiteSpace(cloud.EditorVersion))
        {
            throw new InvalidOperationException("Cloud config requires 'EditorVersion'.");
        }

        return BuildBundles(cloud)
            .Aggregate((a, b) => a.Merge(b))
            .WithMergedJs([AssetsBundle.BlazorIntegrationAsset]);
    }

    private IEnumerable<AssetsBundle> BuildBundles(CloudConfig cloud)
    {
        yield return editorBuilder.Build(cloud.EditorVersion, cloud.CdnUrl);

        if (cloud.Premium)
        {
            yield return premiumBuilder.Build(cloud.EditorVersion, cloud.CdnUrl);
        }

        if (cloud.CKBox is { } ckbox)
        {
            yield return ckboxBuilder.Build(
                ckbox.Version ?? throw new InvalidOperationException("Cloud config requires CKBox 'Version' when CKBox is enabled."),
                ckbox.Translations,
                ckbox.CdnUrl,
                ckbox.Theme ?? "lark");
        }
    }
}
