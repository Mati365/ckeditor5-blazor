using CKEditor.Blazor.Bundle;
using CKEditor.Blazor.Cloud.CKBox;
using CKEditor.Blazor.Cloud.CKEditor;

namespace CKEditor.Blazor.Cloud;

/// <summary>
/// Builds an <see cref="AssetsBundle"/> from a cloud configuration.
/// </summary>
public static class CloudBundleBuilder
{
    /// <summary>
    /// Creates an <see cref="AssetsBundle"/> from the given cloud configuration.
    /// </summary>
    /// <param name="cloud">The cloud configuration.</param>
    /// <returns>The resulting assets bundle.</returns>
    public static AssetsBundle Build(CloudConfig cloud)
    {
        if (string.IsNullOrWhiteSpace(cloud.EditorVersion))
        {
            throw new InvalidOperationException("Cloud config requires 'EditorVersion'.");
        }

        var editorBundle = CKEditorCloudBundleBuilder.Build(cloud.EditorVersion, cloud.CdnUrl);

        if (cloud.Premium)
        {
            var premiumBundle = CKEditorPremiumCloudBundleBuilder.Build(cloud.EditorVersion, cloud.CdnUrl);
            editorBundle = editorBundle.Merge(premiumBundle);
        }

        if (cloud.CKBox is not null)
        {
            if (string.IsNullOrWhiteSpace(cloud.CKBox.Version))
            {
                throw new InvalidOperationException("Cloud config requires CKBox 'Version' when CKBox is enabled.");
            }

            var ckboxBundle = CKBoxCloudBundleBuilder.Build(
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
