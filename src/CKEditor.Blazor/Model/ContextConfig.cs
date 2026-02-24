namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents a CKEditor context configuration.
/// </summary>
public sealed record ContextConfig
{
    /// <summary>
    /// The context configuration object.
    /// </summary>
    public Dictionary<string, object> Config { get; init; } = [];

    /// <summary>
    /// Plugins to be loaded in the context.
    /// </summary>
    public List<string> Plugins { get; init; } = [];
}
