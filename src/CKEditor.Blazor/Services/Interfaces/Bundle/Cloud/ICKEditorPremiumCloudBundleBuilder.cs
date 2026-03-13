using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;

/// <summary>
/// Builds the set of assets (JavaScript and CSS) required to load CKEditor 5 Premium Features from the CKEditor Cloud.
/// </summary>
public interface ICKEditorPremiumCloudBundleBuilder
{
    /// <summary>
    /// Builds an <see cref="AssetsBundle"/> for CKEditor 5 Premium Features.
    /// </summary>
    /// <param name="version">The version of the premium features package to load.</param>
    /// <param name="cdnUrl">The base URL of the CDN.</param>
    /// <returns>The resulting <see cref="AssetsBundle"/>.</returns>
    AssetsBundle Build(string version, string cdnUrl);
}
