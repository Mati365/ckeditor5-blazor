using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;

/// <summary>
/// Builds the set of assets (JavaScript and CSS) required to load CKEditor 5 Premium Features from a self-hosted installation.
/// </summary>
public interface ICKEditorPremiumSelfHostedBundleBuilder
{
    /// <summary>
    /// Builds an <see cref="AssetsBundle"/> for self-hosted CKEditor 5 Premium Features.
    /// </summary>
    /// <param name="version">The version of the premium features package to load.</param>
    /// <param name="basePath">The base path where the self-hosted assets are hosted.</param>
    /// <returns>The resulting <see cref="AssetsBundle"/>.</returns>
    AssetsBundle Build(string version, string basePath);
}
