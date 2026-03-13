using CKEditor.Blazor.Exceptions;
using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.Cloud;
using CKEditor.Blazor.Services.Interfaces.Bundle;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;

namespace CKEditor.Blazor.Services.Bundle.Cloud;

/// <summary>
/// Builds an <see cref="AssetsBundle"/> from a cloud configuration.
/// </summary>
public class CloudBundleBuilder(
    ICKEditorCloudBundleBuilder editorBuilder,
    ICKEditorPremiumCloudBundleBuilder premiumBuilder,
    ICKBoxCloudBundleBuilder ckboxBuilder,
    IBlazorIntegrationBundleBuilder blazorIntegrationAssetFactory) : ICloudBundleBuilder
{
    public AssetsBundle Build(CloudConfig cloud)
    {
        if (string.IsNullOrWhiteSpace(cloud.EditorVersion))
        {
            throw new CloudConfigurationException("Cloud config requires 'EditorVersion'.");
        }

        return BuildBundles(cloud).Aggregate((a, b) => a.Merge(b));
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
                ckbox.Version ?? throw new CloudConfigurationException("Cloud config requires CKBox 'Version' when CKBox is enabled."),
                ckbox.Translations,
                ckbox.CdnUrl,
                ckbox.Theme ?? "lark");
        }

        yield return blazorIntegrationAssetFactory.Build(cloud.IntegrationBasePath);
    }
}
