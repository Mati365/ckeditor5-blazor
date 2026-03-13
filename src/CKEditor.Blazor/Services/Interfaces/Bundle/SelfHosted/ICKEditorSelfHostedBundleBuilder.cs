using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;

/// <summary>
/// Builds the set of assets (JavaScript and CSS) required to load CKEditor 5 from a self-hosted installation.
/// </summary>
public interface ICKEditorSelfHostedBundleBuilder
{
    /// <summary>
    /// Builds an <see cref="AssetsBundle"/> for self-hosted CKEditor 5.
    /// </summary>
    /// <param name="version">The CKEditor 5 version to load.</param>
    /// <param name="basePath">The base path where the self-hosted assets are hosted.</param>
    /// <returns>The resulting <see cref="AssetsBundle"/>.</returns>
    AssetsBundle Build(string version, string basePath);
}
