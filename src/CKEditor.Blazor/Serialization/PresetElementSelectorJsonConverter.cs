using System.Text.Json;
using System.Text.Json.Serialization;
using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Serialization;

/// <summary>
/// JSON converter for <see cref="PresetElementSelector"/>.
/// Serializes and deserializes the value as <c>{ "$element": "selector" }</c>.
/// </summary>
public sealed class PresetElementSelectorJsonConverter : JsonConverter<PresetElementSelector>
{
    /// <summary>
    /// Reads a <see cref="PresetElementSelector"/> from a JSON object containing a <c>$element</c> property.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert (should be <see cref="PresetElementSelector"/>).</param>
    /// <param name="options">The serialization options to use.</param>
    /// <exception cref="JsonException">Thrown when the expected <c>$element</c> property is missing.</exception>
    /// <returns>A <see cref="PresetElementSelector"/> with the selector from the JSON.</returns>
    public override PresetElementSelector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.TryGetProperty("$element", out var element))
        {
            return new PresetElementSelector(element.GetString()!);
        }

        throw new JsonException("Expected object with '$element' property.");
    }

    /// <summary>
    /// Writes a <see cref="PresetElementSelector"/> as a JSON object with a <c>$element</c> property.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="PresetElementSelector"/> to write.</param>
    /// <param name="options">The serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, PresetElementSelector value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("$element", value.Selector);
        writer.WriteEndObject();
    }
}
