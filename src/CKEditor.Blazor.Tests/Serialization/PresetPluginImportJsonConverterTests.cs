using System.Text.Json;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Serialization;

namespace CKEditor.Blazor.Tests.Serialization;

public class PresetPluginImportJsonConverterTests
{
    private static readonly JsonSerializerOptions _options = new()
    {
        Converters = { new PresetPluginImportJsonConverter() }
    };

    [Fact]
    public void Read_ValidObject_ShouldDeserializeNameAndPath()
    {
        var json = """{"$import":{"name":"MyPlugin","path":"./my-plugin.js"}}""";

        var result = JsonSerializer.Deserialize<PresetPluginImport>(json, _options);

        Assert.NotNull(result);
        Assert.Equal("MyPlugin", result!.Name);
        Assert.Equal("./my-plugin.js", result!.ImportPath);
    }

    [Fact]
    public void Read_MissingImportProperty_ShouldThrowJsonException()
    {
        var json = """{"wrong_key":{"name":"MyPlugin","path":"./my-plugin.js"}}""";

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<PresetPluginImport>(json, _options));
    }

    [Fact]
    public void Write_ShouldSerializeAsImportObject()
    {
        var plugin = new PresetPluginImport("MyPlugin", "./my-plugin.js");

        var json = JsonSerializer.Serialize(plugin, _options);
        using var doc = JsonDocument.Parse(json);

        var importObj = doc.RootElement.GetProperty("$import");
        Assert.Equal("MyPlugin", importObj.GetProperty("name").GetString());
        Assert.Equal("./my-plugin.js", importObj.GetProperty("path").GetString());
    }

    [Fact]
    public void Write_ShouldHaveOnlyImportProperty()
    {
        var plugin = new PresetPluginImport("MyPlugin", "./my-plugin.js");

        var json = JsonSerializer.Serialize(plugin, _options);
        using var doc = JsonDocument.Parse(json);

        Assert.Single(doc.RootElement.EnumerateObject());
    }

    [Fact]
    public void RoundTrip_ShouldPreserveNameAndPath()
    {
        var original = new PresetPluginImport("AwesomePlugin", "/plugins/awesome.js");

        var json = JsonSerializer.Serialize(original, _options);
        var restored = JsonSerializer.Deserialize<PresetPluginImport>(json, _options);

        Assert.Equal(original.Name, restored!.Name);
        Assert.Equal(original.ImportPath, restored!.ImportPath);
    }
}
