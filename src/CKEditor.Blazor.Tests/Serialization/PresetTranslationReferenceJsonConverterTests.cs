using System.Text.Json;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Serialization;

namespace CKEditor.Blazor.Tests.Serialization;

public class PresetTranslationReferenceJsonConverterTests
{
    private static readonly JsonSerializerOptions _options = new()
    {
        Converters = { new PresetTranslationReferenceJsonConverter() }
    };

    [Fact]
    public void Read_ValidObject_ShouldDeserializeKey()
    {
        var json = """{"$translation":"Save"}""";

        var result = JsonSerializer.Deserialize<PresetTranslationReference>(json, _options);

        Assert.NotNull(result);
        Assert.Equal("Save", result!.Key);
    }

    [Fact]
    public void Read_DifferentKey_ShouldDeserializeCorrectly()
    {
        var json = """{"$translation":"editor.toolbar.bold"}""";

        var result = JsonSerializer.Deserialize<PresetTranslationReference>(json, _options);

        Assert.Equal("editor.toolbar.bold", result!.Key);
    }

    [Fact]
    public void Read_MissingTranslationProperty_ShouldThrowJsonException()
    {
        var json = """{"wrong_key":"Save"}""";

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<PresetTranslationReference>(json, _options));
    }

    [Fact]
    public void Write_ShouldSerializeAsTranslationObject()
    {
        var reference = new PresetTranslationReference("Cancel");

        var json = JsonSerializer.Serialize(reference, _options);
        using var doc = JsonDocument.Parse(json);

        Assert.Equal("Cancel", doc.RootElement.GetProperty("$translation").GetString());
    }

    [Fact]
    public void Write_ShouldNotIncludeExtraProperties()
    {
        var reference = new PresetTranslationReference("Ok");

        var json = JsonSerializer.Serialize(reference, _options);
        using var doc = JsonDocument.Parse(json);

        Assert.Single(doc.RootElement.EnumerateObject());
    }

    [Fact]
    public void RoundTrip_ShouldPreserveKey()
    {
        var original = new PresetTranslationReference("editor.save");

        var json = JsonSerializer.Serialize(original, _options);
        var restored = JsonSerializer.Deserialize<PresetTranslationReference>(json, _options);

        Assert.Equal(original.Key, restored!.Key);
    }
}
