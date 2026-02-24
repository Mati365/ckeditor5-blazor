using System.Text.Json.Serialization;
using CKEditor.Blazor.Serialization;

namespace CKEditor.Blazor.Model.License;

/// <summary>
/// Represents a CKEditor 5 license key.
/// This class parses JWT license tokens and extracts basic information
/// such as distribution channel and expiration date.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LicenseKey"/> class.
/// </remarks>
/// <param name="Raw">Raw JWT token.</param>
/// <param name="DistributionChannel">Distribution channel.</param>
/// <param name="ExpiresAt">License expiration timestamp.</param>
[JsonConverter(typeof(LicenseKeyJsonConverter))]
public sealed record LicenseKey(string Raw, DistributionChannel? DistributionChannel = null, long? ExpiresAt = null)
{
    /// <summary>
    /// Creates a GPL license key instance.
    /// </summary>
    /// <returns>New GPL license key instance.</returns>
    public static LicenseKey OfGPL() =>
        new(
            Raw: "GPL",
            DistributionChannel: License.DistributionChannel.SH);

    /// <summary>
    /// Checks if the license has expired.
    /// </summary>
    /// <returns>True if the license has expired, false otherwise.</returns>
    public bool IsExpired() => ExpiresAt.HasValue && ExpiresAt.Value < DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <summary>
    /// Checks if the license is a GPL license.
    /// </summary>
    /// <returns>True if the license is GPL, false otherwise.</returns>
    public bool IsGPL() => Raw == "GPL";

    /// <summary>
    /// Checks if the license is intended for CKEditor Cloud usage only.
    /// </summary>
    /// <returns>True if the license is for CKEditor Cloud usage only, false otherwise.</returns>
    public bool IsCloudOnly() => DistributionChannel == License.DistributionChannel.Cloud;

    /// <summary>
    /// Checks if the license is intended for self-hosted CKEditor usage only.
    /// </summary>
    /// <returns>>True if the license is for self-hosted usage only, false otherwise.</returns>
    public bool IsSelfHostedOnly() => DistributionChannel == License.DistributionChannel.SH;

    /// <summary>
    /// Checks if the license is compatible with CKEditor Cloud hosting.
    /// </summary>
    /// <returns>>True if the license is compatible with CKEditor Cloud, false otherwise.</returns>
    public bool IsCompatibleWithCloud() =>
        DistributionChannel switch
        {
            null => true,
            License.DistributionChannel.Cloud => true,
            _ => false
        };

    /// <summary>
    /// Checks if the license is compatible with self-hosted CKEditor.
    /// </summary>
    /// <returns>True if the license is compatible with self-hosted CKEditor, false otherwise.</returns>
    public bool IsCompatibleWithSelfHosted() =>
        DistributionChannel switch
        {
            null => true,
            License.DistributionChannel.SH => true,
            _ => false
        };
}
