namespace CKEditor.Blazor.Model.License;

/// <summary>
/// Represents a CKEditor 5 license key distribution channel.
/// </summary>
public enum DistributionChannel
{
    /// <summary>
    /// Self-hosted, imported via npm or yarn.
    /// </summary>
    SH,

    /// <summary>
    /// Cloud, imported via importmap or script tag.
    /// </summary>
    Cloud
}
