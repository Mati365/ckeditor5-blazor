using System.Text.Json;
using CKEditor.Blazor.Model.License;

namespace CKEditor.Blazor.Serialization;

/// <summary>
/// Parser for the <c>distributionChannel</c> license claim.
/// </summary>
public static class DistributionChannelParser
{
    /// <summary>
    /// Parses the <c>distributionChannel</c> claim, which can be <c>null</c>, a single string or an array of strings.
    /// </summary>
    /// <param name="element">The claim value.</param>
    /// <returns>The resolved <see cref="DistributionChannel"/>, or <c>null</c> when none applies.</returns>
    /// <exception cref="ArgumentException">When the claim is a string with an unknown value or has an unsupported JSON type.</exception>
    public static DistributionChannel? Parse(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Null => null,
        JsonValueKind.String => ParseName(element.GetString()!)
            ?? throw new ArgumentException("Invalid distributionChannel in JWT payload"),
        JsonValueKind.Array => ParseArray(element),
        _ => throw new ArgumentException("Invalid distributionChannel in JWT payload")
    };

    /// <summary>
    /// Resolves an array of channel names. Unknown entries are ignored.
    /// When the array contains both <c>cloud</c> and <c>sh</c>, <see cref="DistributionChannel.Cloud"/> takes precedence.
    /// </summary>
    /// <param name="array">JSON array of channel names.</param>
    /// <returns>The resolved <see cref="DistributionChannel"/>, or <c>null</c> if the array contains no known channel.</returns>
    private static DistributionChannel? ParseArray(JsonElement array)
    {
        var hasCloud = false;
        var hasSH = false;

        foreach (var item in array.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.String)
            {
                continue;
            }

            switch (ParseName(item.GetString()!))
            {
                case DistributionChannel.Cloud:
                    hasCloud = true;
                    break;
                case DistributionChannel.SH:
                    hasSH = true;
                    break;
            }
        }

        if (hasCloud)
        {
            return DistributionChannel.Cloud;
        }

        return hasSH ? DistributionChannel.SH : null;
    }

    /// <summary>
    /// Maps a channel name (case-insensitive) to a <see cref="DistributionChannel"/>.
    /// </summary>
    /// <param name="name">Channel name.</param>
    /// <returns>The matching channel, or <c>null</c> when the name is unknown.</returns>
    private static DistributionChannel? ParseName(string name)
    {
        if (name.Equals("cloud", StringComparison.OrdinalIgnoreCase))
        {
            return DistributionChannel.Cloud;
        }

        if (name.Equals("sh", StringComparison.OrdinalIgnoreCase))
        {
            return DistributionChannel.SH;
        }

        return null;
    }
}
