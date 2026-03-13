using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;

/// <summary>
/// Builds the set of assets (JavaScript and CSS) required to load CKBox from the CKEditor Cloud.
/// </summary>
public interface ICKBoxCloudBundleBuilder
{
    /// <summary>
    /// Builds an <see cref="AssetsBundle"/> for CKBox.
    /// </summary>
    /// <param name="version">The CKBox version to load.</param>
    /// <param name="translations">The list of translation keys to include.</param>
    /// <param name="cdnUrl">The base URL of the CDN.</param>
    /// <param name="theme">The CKBox theme to use (defaults to "lark").</param>
    /// <returns>The resulting <see cref="AssetsBundle"/>.</returns>
    AssetsBundle Build(string version, IReadOnlyList<string> translations, string cdnUrl, string theme = "lark");
}
