using System.Text.Json;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Serialization;

namespace CKEditor.Blazor.Tests.Serialization;

public class PresetElementSelectorJsonConverterTests
{
    private static readonly JsonSerializerOptions _options = new()
    {
        Converters = { new PresetElementSelectorJsonConverter() }
    };

    [Fact]
    public void Read_ValidObject_ShouldDeserializeSelector()
    {
        var json = """{"$element":".my-editor"}""";

        var result = JsonSerializer.Deserialize<PresetElementSelector>(json, _options);

        Assert.NotNull(result);
        Assert.Equal(".my-editor", result!.Selector);
    }

    [Fact]
    public void Read_IdSelector_ShouldDeserializeSelector()
    {
        var json = """{"$element":"#editor-container"}""";

        var result = JsonSerializer.Deserialize<PresetElementSelector>(json, _options);

        Assert.Equal("#editor-container", result!.Selector);
    }

    [Fact]
    public void Read_MissingElementProperty_ShouldThrowJsonException()
    {
        var json = """{"wrong_key":".editor"}""";

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<PresetElementSelector>(json, _options));
    }

    [Fact]
    public void Write_ShouldSerializeAsElementObject()
    {
        var selector = new PresetElementSelector(".my-class");

        var json = JsonSerializer.Serialize(selector, _options);
        using var doc = JsonDocument.Parse(json);

        Assert.Equal(".my-class", doc.RootElement.GetProperty("$element").GetString());
    }

    [Fact]
    public void Write_ShouldNotIncludeExtraProperties()
    {
        var selector = new PresetElementSelector("#id");

        var json = JsonSerializer.Serialize(selector, _options);
        using var doc = JsonDocument.Parse(json);

        Assert.Single(doc.RootElement.EnumerateObject());
    }

    [Fact]
    public void RoundTrip_ShouldPreserveSelector()
    {
        var original = new PresetElementSelector(".editor-root");

        var json = JsonSerializer.Serialize(original, _options);
        var restored = JsonSerializer.Deserialize<PresetElementSelector>(json, _options);

        Assert.Equal(original.Selector, restored!.Selector);
    }
}
