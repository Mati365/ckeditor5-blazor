using System.Text.Json;
using System.Text.Json.Serialization;
using CKEditor.Blazor.Model.License;

namespace CKEditor.Blazor.Serialization;

/// <summary>
/// JSON converter for LicenseKey that serializes only the raw string.
/// </summary>
public class LicenseKeyJsonConverter : JsonConverter<LicenseKey>
{
    /// <summary>
    /// Reads a JSON string and parses it into a <see cref="LicenseKey"/> instance.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the JSON string token.</param>
    /// <param name="typeToConvert">Type to convert (ignored).</param>
    /// <param name="options">Serializer options (ignored).</param>
    /// <returns>The parsed <see cref="LicenseKey"/>, or <c>null</c> if the JSON value is <c>null</c>.</returns>
    public override LicenseKey? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value == null ? null : LicenseKeyParser.Parse(value);
    }

    /// <summary>
    /// Writes the provided <see cref="LicenseKey"/> as its raw string value.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="LicenseKey"/> to serialize.</param>
    /// <param name="options">Serializer options (ignored).</param>
    public override void Write(Utf8JsonWriter writer, LicenseKey value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Raw);
}
