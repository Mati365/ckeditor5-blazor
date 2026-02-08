namespace CKEditor.Blazor.Models;

/// <summary>
/// Represents a CKEditor context configuration.
/// </summary>
public class Context
{
    /// <summary>
    /// The context configuration object.
    /// </summary>
    public Dictionary<string, object> Config { get; set; } = [];

    /// <summary>
    /// Plugins to be loaded in the context.
    /// </summary>
    public List<string> Plugins { get; set; } = [];
}
