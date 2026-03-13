using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;

/// <summary>
/// Builds the set of assets (JavaScript and CSS) required to load CKBox from a self-hosted installation.
/// </summary>
public interface ICKBoxSelfHostedBundleBuilder
{
    /// <summary>
    /// Builds an <see cref="AssetsBundle"/> for self-hosted CKBox.
    /// </summary>
    /// <param name="version">The CKBox version to load.</param>
    /// <param name="translations">The list of translation keys to include.</param>
    /// <param name="basePath">The base path where the self-hosted assets are hosted.</param>
    /// <param name="theme">The CKBox theme to use (defaults to "lark").</param>
    /// <returns>The resulting <see cref="AssetsBundle"/>.</returns>
    AssetsBundle Build(string version, IReadOnlyList<string> translations, string basePath, string theme = "lark");
}
