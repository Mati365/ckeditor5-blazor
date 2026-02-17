using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CKEditor.Blazor.Model.License;

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
    /// Parses a license key string and creates a license key instance.
    /// </summary>
    /// <param name="key">License key string (JWT or 'GPL').</param>
    /// <returns>New license key instance.</returns>
    /// <exception cref="ArgumentException">When the key is invalid.</exception>
    public static LicenseKey Parse(string key)
    {
        if (key == "GPL")
        {
            return OfGPL();
        }

        return ParseJWT(key);
    }

    /// <summary>
    /// Tries to parse a license key string.
    /// </summary>
    /// <param name="key">License key string to parse.</param>
    /// <param name="licenseKey">The parsed license key, or null if parsing failed.</param>
    /// <returns>True if parsing succeeded, false otherwise.</returns>
    public static bool TryParse(string? key, out LicenseKey? licenseKey)
    {
        if (string.IsNullOrEmpty(key))
        {
            licenseKey = null;
            return false;
        }

        try
        {
            licenseKey = Parse(key);
            return true;
        }
        catch
        {
            licenseKey = null;
            return false;
        }
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

    /// <summary>
    /// Parses a JWT token and creates a license key instance.
    /// </summary>
    /// <param name="jwt">JWT token to parse.</param>
    /// <returns>New license key instance.</returns>
    /// <exception cref="ArgumentException">When token is empty or invalid.</exception>
    private static LicenseKey ParseJWT(string jwt)
    {
        if (string.IsNullOrEmpty(jwt))
        {
            throw new ArgumentException("License key cannot be empty", nameof(jwt));
        }

        var parts = jwt.Split('.');

        if (parts.Length < 2)
        {
            throw new ArgumentException("Invalid JWT format", nameof(jwt));
        }

        var payload = DecodeJWTPayload(parts[1]);

        DistributionChannel? distributionChannel = null;

        if (payload.TryGetValue("distributionChannel", out var channelElement))
        {
            var channelStr = channelElement.GetString();

            distributionChannel = channelStr?.ToLowerInvariant() switch
            {
                "sh" => License.DistributionChannel.SH,
                "cloud" => License.DistributionChannel.Cloud,
                null => null,
                _ => throw new ArgumentException("Invalid distributionChannel in JWT payload")
            };
        }

        long? expiresAt = null;

        if (payload.TryGetValue("exp", out var expElement))
        {
            expiresAt = expElement.GetInt64();
        }

        return new LicenseKey(
            raw: jwt,
            distributionChannel: distributionChannel,
            expiresAt: expiresAt);
    }

    /// <summary>
    /// Decodes the payload from a JWT token.
    /// </summary>
    /// <param name="encodedPayload">Base64url encoded payload.</param>
    /// <returns>Decoded payload data.</returns>
    /// <exception cref="ArgumentException">When decoding fails.</exception>
    private static Dictionary<string, JsonElement> DecodeJWTPayload(string encodedPayload)
    {
        try
        {
            var base64 = encodedPayload.Replace('-', '+').Replace('_', '/');
            var padLength = 4 - (base64.Length % 4);

            if (padLength < 4)
            {
                base64 += new string('=', padLength);
            }

            var decoded = Convert.FromBase64String(base64);
            var json = Encoding.UTF8.GetString(decoded);
            var payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? throw new ArgumentException("Invalid JSON in JWT payload");

            return payload;
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new ArgumentException("Invalid JWT payload encoding", nameof(encodedPayload), ex);
        }
    }
}
