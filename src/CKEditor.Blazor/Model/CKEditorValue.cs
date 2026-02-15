using System.Text.Json;
using System.Text.Json.Serialization;

namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents the value of a CKEditor 5 instance.
/// Supports both single-root (standard) and multi-root editor configurations.
/// This class is automatically serialized to its internal dictionary format.
/// </summary>
[JsonConverter(typeof(CKEditorValueConverter))]
public class CKEditorValue
{
    private readonly Dictionary<string, string> _roots;

    /// <summary>
    /// Initializes a new instance of the <see cref="CKEditorValue"/> class using a single string value.
    /// Maps the content to the "main" root.
    /// </summary>
    /// <param name="value">The HTML content for the 'main' editable area.</param>
    public CKEditorValue(string? value)
    {
        _roots = string.IsNullOrEmpty(value)
            ? []
            : new Dictionary<string, string> { { "main", value } };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CKEditorValue"/> class using a dictionary of roots.
    /// Used for Multi-root editors where multiple editable areas exist.
    /// </summary>
    /// <param name="roots">A dictionary where keys are root names and values are their respective HTML content.</param>
    public CKEditorValue(Dictionary<string, string>? roots)
    {
        _roots = roots ?? [];
    }

    /// <summary>
    /// Gets the underlying dictionary of editor roots and their content.
    /// </summary>
    public IReadOnlyDictionary<string, string> Data => _roots;

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a <see cref="CKEditorValue"/>.
    /// </summary>
    /// <param name="value">The string content to be used as the value for the 'main' root.</param>
    public static implicit operator CKEditorValue(string? value)
    {
        return new(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="Dictionary{TKey, TValue}"/> to a <see cref="CKEditorValue"/>.
    /// </summary>
    /// <param name="value">The dictionary of root names and their respective content.</param>
    public static implicit operator CKEditorValue(Dictionary<string, string>? value)
    {
        return new(value);
    }

    /// <summary>
    /// Internal converter to ensure that only the <see cref="Data"/> dictionary
    /// is serialized, making it compatible with CKEditor 5 JS expectations.
    /// </summary>
    private class CKEditorValueConverter : JsonConverter<CKEditorValue>
    {
        public override CKEditorValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options);
            return new CKEditorValue(dictionary);
        }

        public override void Write(Utf8JsonWriter writer, CKEditorValue value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Data, options);
        }
    }
}
