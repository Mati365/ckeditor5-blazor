using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Tests.Model;

public class EditorValueTests
{
    [Fact]
    public void EditorValue_Construct_FromString_ShouldMapToMainRoot()
    {
        var content = "test content";

        var editorValue = new EditorValue(content);

        Assert.Single(editorValue.Data);
        Assert.Equal(content, editorValue.Data["main"]);
    }

    [Fact]
    public void EditorValue_Construct_FromNullString_ShouldMapEmptyToMainRoot()
    {
        var editorValue = new EditorValue((string?)null);

        Assert.Single(editorValue.Data);
        Assert.Equal(string.Empty, editorValue.Data["main"]);
    }

    [Fact]
    public void EditorValue_Construct_FromDictionary_ShouldStoreDictionary()
    {
        var roots = new Dictionary<string, string>
        {
            { "main", "content1" },
            { "sidebar", "content2" }
        };

        var editorValue = new EditorValue(roots);

        Assert.Equal(2, editorValue.Data.Count);
        Assert.Equal("content1", editorValue.Data["main"]);
        Assert.Equal("content2", editorValue.Data["sidebar"]);
    }

    [Fact]
    public void EditorValue_Construct_FromNullDictionary_ShouldStoreEmptyDictionary()
    {
        var editorValue = new EditorValue((Dictionary<string, string>?)null);

        Assert.Empty(editorValue.Data);
    }

    [Fact]
    public void EditorValue_ImplicitConversion_FromString_Works()
    {
        var str = "content";

        EditorValue editorValue = str;

        Assert.Single(editorValue.Data);
        Assert.Equal(str, editorValue.Data["main"]);
    }

    [Fact]
    public void EditorValue_ImplicitConversion_FromDictionary_Works()
    {
        var dict = new Dictionary<string, string> { { "head", "val" } };

        EditorValue editorValue = dict;

        Assert.Single(editorValue.Data);
        Assert.Equal("val", editorValue.Data["head"]);
    }

    [Fact]
    public void EditorValue_ImplicitConversion_FromNullDictionary_Works()
    {
        EditorValue editorValue = (Dictionary<string, string>?)null;

        Assert.Empty(editorValue.Data);
    }
}
