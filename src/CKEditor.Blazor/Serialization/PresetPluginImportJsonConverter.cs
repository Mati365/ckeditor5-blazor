using System.Text.Json;
using System.Text.Json.Serialization;
using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Serialization;

/// <summary>
/// JSON converter for <see cref="PresetPluginImport"/>.
/// Serializes and deserializes the value as <c>{ "$import": { "name": "...", "path": "..." } }</c>.
/// </summary>
public sealed class PresetPluginImportJsonConverter : JsonConverter<PresetPluginImport>
{
    /// <summary>
    /// Reads a <see cref="PresetPluginImport"/> from a JSON object containing a <c>$import</c> property.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert (should be <see cref="PresetPluginImport"/>).</param>
    /// <param name="options">The serialization options to use.</param>
    /// <exception cref="JsonException">Thrown when the expected <c>$import</c> property is missing.</exception>
    /// <returns>A <see cref="PresetPluginImport"/> with the name and path from the JSON.</returns>
    public override PresetPluginImport Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.TryGetProperty("$import", out var importObj))
        {
            var name = importObj.GetProperty("name").GetString()!;
            var path = importObj.GetProperty("path").GetString()!;

            return new PresetPluginImport(name, path);
        }

        throw new JsonException("Expected object with '$import' property containing 'name' and 'path'.");
    }

    /// <summary>
    /// Writes a <see cref="PresetPluginImport"/> as a JSON object with a <c>$import</c> property.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="PresetPluginImport"/> to write.</param>
    /// <param name="options">The serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, PresetPluginImport value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("$import");
        writer.WriteStartObject();
        writer.WriteString("name", value.Name);
        writer.WriteString("path", value.ImportPath);
        writer.WriteEndObject();
        writer.WriteEndObject();
    }
}
