using System.Text.Json;
using System.Text.Json.Serialization;

namespace CKEditor.Blazor.License;

/// <summary>
/// JSON converter for LicenseKey that serializes only the raw string.
/// </summary>
public class LicenseKeyJsonConverter : JsonConverter<LicenseKey>
{
    public override LicenseKey? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value == null ? null : LicenseKey.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, LicenseKey value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Raw);
    }
}
