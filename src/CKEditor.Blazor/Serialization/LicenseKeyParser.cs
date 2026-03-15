using System.Text;
using System.Text.Json;
using CKEditor.Blazor.Model.License;

namespace CKEditor.Blazor.Serialization;

/// <summary>
/// Parser for `LicenseKey` strings (JWT or "GPL").
/// </summary>
public static class LicenseKeyParser
{
    /// <summary>
    /// Parses a license key string and returns a <see cref="LicenseKey"/>.
    /// Supports the special value "GPL" or a JWT token containing the license claims.
    /// </summary>
    /// <param name="key">License key string (JWT or "GPL").</param>
    /// <returns>The parsed <see cref="LicenseKey"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided JWT is invalid.</exception>
    public static LicenseKey Parse(string? key) => key is null or "GPL" ? LicenseKey.OfGPL() : ParseJWT(key);

    /// <summary>
    /// Parses a JWT license token and extracts known claims (distribution channel and expiry).
    /// </summary>
    /// <param name="jwt">JWT token to parse.</param>
    /// <returns>A <see cref="LicenseKey"/> populated from JWT claims.</returns>
    /// <exception cref="ArgumentException">If the token is empty or has an invalid format/claims.</exception>
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
                "sh" => DistributionChannel.SH,
                "cloud" => DistributionChannel.Cloud,
                null => null,
                _ => throw new ArgumentException("Invalid distributionChannel in JWT payload")
            };
        }

        long? expiresAt = null;

        if (payload.TryGetValue("exp", out var expElement))
        {
            expiresAt = expElement.GetInt64();
        }

        return new LicenseKey(jwt, distributionChannel, expiresAt);
    }

    /// <summary>
    /// Decodes the base64url-encoded JWT payload and deserializes it to a dictionary of claims.
    /// </summary>
    /// <param name="encodedPayload">Base64url-encoded payload part of a JWT.</param>
    /// <returns>Dictionary of claim names to <see cref="JsonElement"/> values.</returns>
    /// <exception cref="ArgumentException">When decoding or JSON parsing fails.</exception>
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
