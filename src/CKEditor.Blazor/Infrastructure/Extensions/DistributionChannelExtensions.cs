using CKEditor.Blazor.Model.License;

namespace CKEditor.Blazor.Infrastructure;

/// <summary>
/// Extension methods for DistributionChannel.
/// </summary>
public static class DistributionChannelExtensions
{
    /// <summary>
    /// Checks if this distribution channel is compatible with another.
    /// </summary>
    /// <param name="channel">The current distribution channel.</param>
    /// <param name="other">The other distribution channel to compare with. If null, it's considered compatible.</param>
    /// <returns>True if compatible, false otherwise.</returns>
    public static bool IsCompatibleWith(this DistributionChannel channel, DistributionChannel? other)
    {
        return other == null || channel == other;
    }
}
