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
    /// <returns>The resolved <see cref="DistributionChannel"/>, or <c>null</c> when every channel is allowed.</returns>
    /// <exception cref="ArgumentException">When the claim has an unknown value, contains no known channel or has an unsupported JSON type.</exception>
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
    /// </summary>
    /// <param name="array">JSON array of channel names.</param>
    /// <returns>
    /// The single listed <see cref="DistributionChannel"/>, or <c>null</c> (every channel allowed)
    /// when the array contains both <c>cloud</c> and <c>sh</c>.
    /// </returns>
    /// <exception cref="ArgumentException">When the array contains no known channel.</exception>
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

        return (hasCloud, hasSH) switch
        {
            (true, true) => null,
            (true, false) => DistributionChannel.Cloud,
            (false, true) => DistributionChannel.SH,
            (false, false) => throw new ArgumentException("Invalid distributionChannel in JWT payload")
        };
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
