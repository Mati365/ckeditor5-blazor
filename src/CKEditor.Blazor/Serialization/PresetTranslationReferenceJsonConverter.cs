using System.Text.Json;
using System.Text.Json.Serialization;
using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Serialization;

/// <summary>
/// JSON converter for <see cref="PresetTranslationReference"/>.
/// Serializes and deserializes the value as <c>{ "$translation": "key" }</c>.
/// </summary>
public sealed class PresetTranslationReferenceJsonConverter : JsonConverter<PresetTranslationReference>
{
    /// <summary>
    /// Reads a <see cref="PresetTranslationReference"/> from a JSON object containing a <c>$translation</c> property.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert (should be <see cref="PresetTranslationReference"/>).</param>
    /// <param name="options">The serialization options to use.</param>
    /// <exception cref="JsonException">Thrown when the expected <c>$translation</c> property is missing.</exception>
    /// <returns>A <see cref="PresetTranslationReference"/> with the key from the JSON.</returns>
    public override PresetTranslationReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.TryGetProperty("$translation", out var translation))
        {
            return new PresetTranslationReference(translation.GetString()!);
        }

        throw new JsonException("Expected object with '$translation' property.");
    }

    /// <summary>
    /// Writes a <see cref="PresetTranslationReference"/> as a JSON object with a <c>$translation</c> property.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="PresetTranslationReference"/> to write.</param>
    /// <param name="options">The serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, PresetTranslationReference value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("$translation", value.Key);
        writer.WriteEndObject();
    }
}
