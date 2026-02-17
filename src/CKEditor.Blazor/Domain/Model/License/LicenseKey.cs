using System.Text.Json.Serialization;
using CKEditor.Blazor.Infrastructure;

namespace CKEditor.Blazor.Domain.Model.License;

/// <summary>
/// Represents a CKEditor 5 license key.
/// This class parses JWT license tokens and extracts basic information
/// such as distribution channel and expiration date.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LicenseKey"/> class.
/// </remarks>
/// <param name="raw">Raw JWT token.</param>
/// <param name="distributionChannel">Distribution channel.</param>
/// <param name="expiresAt">License expiration timestamp.</param>
[JsonConverter(typeof(LicenseKeyJsonConverter))]
public sealed class LicenseKey(string raw, DistributionChannel? distributionChannel = null, long? expiresAt = null)
{
    /// <summary>
    /// Gets the raw JWT token.
    /// </summary>
    public string Raw { get; init; } = raw;

    /// <summary>
    /// Gets the distribution channel (e.g., 'npm', 'cdn').
    /// </summary>
    public DistributionChannel? DistributionChannel { get; init; } = distributionChannel;

    /// <summary>
    /// Gets the license expiration timestamp.
    /// </summary>
    public long? ExpiresAt { get; init; } = expiresAt;

    /// <summary>
    /// Creates a GPL license key instance.
    /// </summary>
    /// <returns>New GPL license key instance.</returns>
    public static LicenseKey OfGPL()
    {
        return new LicenseKey(
            raw: "GPL",
            distributionChannel: License.DistributionChannel.SH);
    }

    /// <summary>
    /// Checks if the license has expired.
    /// </summary>
    /// <returns>True if the license has expired, false otherwise.</returns>
    public bool IsExpired()
    {
        return ExpiresAt.HasValue && ExpiresAt.Value < DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    /// <summary>
    /// Checks if the license is a GPL license.
    /// </summary>
    /// <returns>True if the license is GPL, false otherwise.</returns>
    public bool IsGPL()
    {
        return Raw == "GPL";
    }
}
