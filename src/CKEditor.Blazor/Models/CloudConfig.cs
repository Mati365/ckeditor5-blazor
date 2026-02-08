namespace CKEditor.Blazor.Models;

/// <summary>
/// Represents cloud configuration for CKEditor.
/// </summary>
public class CloudConfig
{
    /// <summary>
    /// The bundle version to use.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// The bundle ID.
    /// </summary>
    public string? BundleId { get; set; }

    /// <summary>
    /// Additional cloud options.
    /// </summary>
    public Dictionary<string, object> Options { get; set; } = [];
}
