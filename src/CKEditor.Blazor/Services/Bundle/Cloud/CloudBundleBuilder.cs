using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.Cloud;

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

        var editorBundle = editorBuilder.Build(cloud.EditorVersion, cloud.CdnUrl);

        if (cloud.Premium)
        {
            var premiumBundle = premiumBuilder.Build(cloud.EditorVersion, cloud.CdnUrl);
            editorBundle = editorBundle.Merge(premiumBundle);
        }

        if (cloud.CKBox is not null)
        {
            if (string.IsNullOrWhiteSpace(cloud.CKBox.Version))
            {
                throw new InvalidOperationException("Cloud config requires CKBox 'Version' when CKBox is enabled.");
            }

            var ckboxBundle = ckboxBuilder.Build(
                cloud.CKBox.Version,
                cloud.CKBox.Translations,
                cloud.CKBox.CdnUrl,
                cloud.CKBox.Theme ?? "lark");

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
