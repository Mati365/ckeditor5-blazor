namespace CKEditor.Blazor.Exceptions;

/// <summary>
/// Represents a generic configuration problem within the CKEditor.Blazor library.
/// Specific configuration domains (cloud, self‑hosted, etc.) can derive from
/// this type to provide a more accurate exception hierarchy.
/// </summary>
public class ConfigurationException : CKEditorException
{
    public ConfigurationException()
    {
    }

    public ConfigurationException(string message)
        : base(message)
    {
    }

    public ConfigurationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
