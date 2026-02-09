using System.Text.Json;
using System.Text.Json.Serialization;

namespace CKEditor.Blazor.Preset;

/// <summary>
/// JSON converter for EditorType that serializes as string value.
/// </summary>
internal sealed class EditorTypeJsonConverter : JsonConverter<EditorType>
{
    public override EditorType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            return value != null ? EditorType.Parse(value) : null;
        }

        throw new JsonException("Expected string value for EditorType");
    }

    public override void Write(Utf8JsonWriter writer, EditorType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
