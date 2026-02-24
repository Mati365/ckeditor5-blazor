using CKEditor.Blazor.Exceptions;
using CKEditor.Blazor.Model.Cloud;
using CKEditor.Blazor.Model.SelfHosted;

namespace CKEditor.Blazor.Model;

/// <summary>
/// Helper methods that validate a <see cref="PresetConfig"/> before it is
/// consumed by various components.  By convention these methods end with
/// "OrThrow" to mirror existing APIs on <see cref="Services.ConfigManager"/>
/// (e.g. <c>ResolvePresetOrThrow</c>).
/// </summary>
public static class PresetConfigLicenseCompatibilityExtensions
{
    /// <summary>
    /// Ensures that the provided preset contains a cloud configuration and that
    /// the associated license key is compatible with it.  If one of the checks
    /// fails an exception is thrown describing the problem.
    /// </summary>
    /// <param name="preset">The preset to validate.</param>
    /// <returns>The non-null <see cref="CloudConfig"/> stored in <paramref name="preset"/>.
    /// This allows callers to continue working with the configuration without
    /// having to repeat the null check.</returns>
    public static CloudConfig EnsureCloudCompatibilityOrThrow(this PresetConfig preset)
    {
        if (preset.Cloud is null)
        {
            throw new CloudConfigurationMissingException();
        }

        // check license compatibility in the same order that the components
        // previously performed the checks.  The messages are copied verbatim
        // so consumers of the public API behaviour remains unaffected.
        if (!preset.Cloud.HasOfficialCdn() && preset.LicenseKey.IsCloudOnly())
        {
            throw new CloudLicenseIncompatibleException(
                "The license key associated with the preset is only valid for CKEditor Cloud CDN hosting, " +
                "but the CDN URL in the cloud configuration does not appear to be the official CKEditor CDN. " +
                "Please update the CDN URL to point to the official CKEditor CDN or switch to a compatible " +
                "license key that allows usage with custom CDNs.");
        }

        if (preset.Cloud.HasOfficialCdn() && !preset.LicenseKey.IsCompatibleWithCloud())
        {
            throw new CloudLicenseIncompatibleException(
                "The license key associated with the preset is not compatible with CKEditor Cloud CDN hosting. " +
                "Please ensure that the preset's license key is valid for cloud usage or switch to a different CDN hosting option.");
        }

        return preset.Cloud;
    }

    /// <summary>
    /// Validates that the preset's license is compatible with self-hosted
    /// usage.  The method name ends with "OrThrow" to keep it consistent with
    /// other helper methods in the codebase.
    /// </summary>
    /// <param name="preset">The preset to validate.</param>
    /// <returns>The <see cref="SelfHostedConfig"/> associated with the preset.
    /// The config is returned simply for convenience.</returns>
    public static SelfHostedConfig EnsureSelfHostedCompatibilityOrThrow(this PresetConfig preset)
    {
        if (!preset.LicenseKey.IsCompatibleWithSelfHosted())
        {
            throw new SelfHostedLicenseIncompatibleException(
                "The license key associated with the preset is not compatible with self-hosted CKEditor usage. " +
                "Please ensure that the preset's license key is valid for self-hosted usage or remove the cloud configuration from the preset.");
        }

        return preset.SelfHosted;
    }
}
