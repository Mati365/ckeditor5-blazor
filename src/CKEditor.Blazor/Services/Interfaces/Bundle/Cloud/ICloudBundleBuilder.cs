using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.Cloud;

namespace CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;

/// <summary>
/// Builds an <see cref="AssetsBundle"/> from a <see cref="CloudConfig"/> configuration.
/// </summary>
public interface ICloudBundleBuilder
{
    /// <summary>
    /// Builds a combined <see cref="AssetsBundle"/> based on the provided cloud configuration.
    /// </summary>
    /// <param name="cloud">The cloud configuration used to generate the bundle.</param>
    /// <returns>The resulting <see cref="AssetsBundle"/>.</returns>
    AssetsBundle Build(CloudConfig cloud);
}
