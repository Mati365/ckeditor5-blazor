using System.Text.Json;
using System.Text.Json.Serialization;
using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Serialization;

/// <summary>
/// JSON converter for <see cref="EditorValue"/> (moved from nested class in Model).
/// </summary>
public class EditorValueConverter : JsonConverter<EditorValue>
{
    /// <summary>
    /// Deserializes JSON into an <see cref="EditorValue"/> instance.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">Type to convert (ignored).</param>
    /// <param name="options">Serializer options passed to <see cref="JsonSerializer"/>.</param>
    /// <returns>A new <see cref="EditorValue"/> containing the deserialized dictionary.</returns>
    public override EditorValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options);
        return new EditorValue(dictionary);
    }

    /// <summary>
    /// Serializes an <see cref="EditorValue"/> by writing its underlying dictionary.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="EditorValue"/> to serialize.</param>
    /// <param name="options">Serializer options passed to <see cref="JsonSerializer"/>.</param>
    public override void Write(Utf8JsonWriter writer, EditorValue value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Data, options);
    }
}
