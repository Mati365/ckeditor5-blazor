using System.Text.Json;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Serialization;

namespace CKEditor.Blazor.Tests.Serialization;

public class EditorValueConverterTests
{
    private static readonly JsonSerializerOptions _options = new()
    {
        Converters = { new EditorValueConverter() }
    };

    [Fact]
    public void Read_SingleRootObject_ShouldDeserializeCorrectly()
    {
        var json = """{"main":"<p>Hello</p>"}""";

        var result = JsonSerializer.Deserialize<EditorValue>(json, _options);

        Assert.NotNull(result);
        Assert.Single(result!.Data);
        Assert.Equal("<p>Hello</p>", result.Data["main"]);
    }

    [Fact]
    public void Read_MultipleRoots_ShouldDeserializeAllEntries()
    {
        var json = """{"main":"content1","sidebar":"content2"}""";

        var result = JsonSerializer.Deserialize<EditorValue>(json, _options);

        Assert.NotNull(result);
        Assert.Equal(2, result!.Data.Count);
        Assert.Equal("content1", result.Data["main"]);
        Assert.Equal("content2", result.Data["sidebar"]);
    }

    [Fact]
    public void Read_EmptyObject_ShouldReturnEmptyEditorValue()
    {
        var json = "{}";

        var result = JsonSerializer.Deserialize<EditorValue>(json, _options);

        Assert.NotNull(result);
        Assert.Empty(result!.Data);
    }

    [Fact]
    public void Read_NullValue_ShouldReturnNull()
    {
        // System.Text.Json short-circuits null tokens before calling the converter
        // for nullable reference types — the result is null by framework design.
        var json = "null";

        var result = JsonSerializer.Deserialize<EditorValue>(json, _options);

        Assert.Null(result);
    }

    [Fact]
    public void Write_SingleRoot_ShouldSerializeAsDictionary()
    {
        var editorValue = new EditorValue("<p>Test</p>");

        var json = JsonSerializer.Serialize(editorValue, _options);
        using var doc = JsonDocument.Parse(json);

        Assert.Equal("<p>Test</p>", doc.RootElement.GetProperty("main").GetString());
    }

    [Fact]
    public void Write_MultipleRoots_ShouldSerializeAllKeys()
    {
        var roots = new Dictionary<string, string>
        {
            { "main", "A" },
            { "header", "B" }
        };
        var editorValue = new EditorValue(roots);

        var json = JsonSerializer.Serialize(editorValue, _options);
        using var doc = JsonDocument.Parse(json);

        Assert.Equal("A", doc.RootElement.GetProperty("main").GetString());
        Assert.Equal("B", doc.RootElement.GetProperty("header").GetString());
    }

    [Fact]
    public void RoundTrip_ShouldPreserveAllRoots()
    {
        var original = new EditorValue(new Dictionary<string, string>
        {
            { "main", "hello" },
            { "secondary", "world" }
        });

        var json = JsonSerializer.Serialize(original, _options);
        var restored = JsonSerializer.Deserialize<EditorValue>(json, _options);

        Assert.Equal(original.Data["main"], restored!.Data["main"]);
        Assert.Equal(original.Data["secondary"], restored.Data["secondary"]);
    }
}
