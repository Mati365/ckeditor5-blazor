using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.SelfHosted;

namespace CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;

/// <summary>
/// Builds an <see cref="AssetsBundle"/> from a self-hosted configuration.
/// </summary>
public interface ISelfHostedBundleBuilder
{
    /// <summary>
    /// Builds a combined <see cref="AssetsBundle"/> based on the provided self-hosted configuration.
    /// </summary>
    /// <param name="selfHosted">The self-hosted configuration used to generate the bundle.</param>
    /// <returns>The resulting <see cref="AssetsBundle"/>.</returns>
    AssetsBundle Build(SelfHostedConfig selfHosted);
}
