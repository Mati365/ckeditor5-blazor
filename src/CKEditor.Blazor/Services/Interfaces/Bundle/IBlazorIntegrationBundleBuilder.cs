using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Interfaces.Bundle;

/// <summary>
/// Generates assets bundles used to integrate the CKEditor Blazor component.
/// </summary>
public interface IBlazorIntegrationBundleBuilder
{
    /// <summary>
    /// Creates the assets bundle that includes the JS asset for the Blazor integration script.
    /// </summary>
    /// <param name="integrationBasePath">The base path where the Blazor integration script is hosted.</param>
    /// <returns>An <see cref="AssetsBundle"/> containing the Blazor integration assets.</returns>
    AssetsBundle Build(string integrationBasePath);
}
