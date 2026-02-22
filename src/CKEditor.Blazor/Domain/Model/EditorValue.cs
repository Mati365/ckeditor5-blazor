using System.Text.Json.Serialization;
using CKEditor.Blazor.Infrastructure;

namespace CKEditor.Blazor.Domain.Model;

/// <summary>
/// Represents the value of a CKEditor 5 instance.
/// Supports both single-root (standard) and multi-root editor configurations.
/// This class is automatically serialized to its internal dictionary format.
/// </summary>
[JsonConverter(typeof(EditorValueConverter))]
public class EditorValue
{
    private readonly Dictionary<string, string> _roots;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorValue"/> class using a single string value.
    /// Maps the content to the "main" root.
    /// </summary>
    /// <param name="value">The HTML content for the 'main' editable area.</param>
    public EditorValue(string? value)
    {
        _roots = new Dictionary<string, string> { { "main", value ?? string.Empty } };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorValue"/> class using a dictionary of roots.
    /// Used for Multi-root editors where multiple editable areas exist.
    /// </summary>
    /// <param name="roots">A dictionary where keys are root names and values are their respective HTML content.</param>
    public EditorValue(Dictionary<string, string>? roots)
    {
        _roots = roots ?? [];
    }

    /// <summary>
    /// Gets the underlying dictionary of editor roots and their content.
    /// </summary>
    public IReadOnlyDictionary<string, string> Data => _roots;

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="EditorValue"/>.
    /// </summary>
    /// <param name="value">The string content to be used as the value for the 'main' root.</param>
    public static implicit operator EditorValue(string? value)
    {
        return new(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="Dictionary{TKey, TValue}"/> to a <see cref="EditorValue"/>.
    /// </summary>
    /// <param name="value">The dictionary of root names and their respective content.</param>
    public static implicit operator EditorValue(Dictionary<string, string>? value)
    {
        return new(value);
    }
}
