using System.Text;
using System.Text.Json;

namespace CKEditor.Blazor.Tests.Helpers;

/// <summary>
/// Helper for generating fake but structurally valid JWT tokens to use in tests.
/// The tokens are unsigned (empty signature), which is sufficient because
/// <c>LicenseKeyParser</c> only decodes the payload — it never verifies the signature.
/// </summary>
public static class JwtTestHelper
{
    private static readonly string _header = Base64UrlEncode(
        """{"alg":"HS256","typ":"JWT"}"""
    );

    /// <summary>
    /// Builds a JWT token string from the provided claims.
    /// </summary>
    /// <param name="distributionChannel">Optional <c>distributionChannel</c> claim value (e.g. "sh", "cloud").</param>
    /// <param name="exp">Optional Unix timestamp for the <c>exp</c> claim.</param>
    /// <param name="extraClaims">Any additional raw key/value claims merged into the payload.</param>
    /// <returns>A dot-separated three-part JWT string.</returns>
    public static string Build(
        string? distributionChannel = null,
        long? exp = null,
        Dictionary<string, object?>? extraClaims = null)
    {
        var baseClaims = new Dictionary<string, object?>
        {
            ["iss"] = "test",
            ["distributionChannel"] = distributionChannel,
            ["exp"] = exp
        }.Where(static kvp => kvp.Value is not null);

        var payload = baseClaims
            .Concat(extraClaims ?? [])
            .GroupBy(static kvp => kvp.Key)
            .ToDictionary(static g => g.Key, static g => g.Last().Value);

        var payloadJson = JsonSerializer.Serialize(payload);
        var payloadEncoded = Base64UrlEncode(payloadJson);

        return $"{_header}.{payloadEncoded}.fake-signature";
    }

    /// <summary>
    /// Builds a JWT token that is already expired (<c>exp</c> set to one day in the past).
    /// </summary>
    public static string BuildExpired(string? distributionChannel = null)
        => Build(distributionChannel, DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds());

    /// <summary>
    /// Builds a JWT token that is valid and expires one day in the future.
    /// </summary>
    public static string BuildValid(string? distributionChannel = null)
        => Build(distributionChannel, DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds());

    private static string Base64UrlEncode(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}
