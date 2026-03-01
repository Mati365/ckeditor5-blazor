using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Tests.Model;

public class EditorValueTests
{
    [Fact]
    public void EditorValue_Construct_FromString_ShouldMapToMainRoot()
    {
        // Arrange
        var content = "test content";

        // Act
        var editorValue = new EditorValue(content);

        // Assert
        Assert.Single(editorValue.Data);
        Assert.Equal(content, editorValue.Data["main"]);
    }

    [Fact]
    public void EditorValue_Construct_FromNullString_ShouldMapEmptyToMainRoot()
    {
        // Act
        var editorValue = new EditorValue((string?)null);

        // Assert
        Assert.Single(editorValue.Data);
        Assert.Equal(string.Empty, editorValue.Data["main"]);
    }

    [Fact]
    public void EditorValue_Construct_FromDictionary_ShouldStoreDictionary()
    {
        // Arrange
        var roots = new Dictionary<string, string>
        {
            { "main", "content1" },
            { "sidebar", "content2" }
        };

        // Act
        var editorValue = new EditorValue(roots);

        // Assert
        Assert.Equal(2, editorValue.Data.Count);
        Assert.Equal("content1", editorValue.Data["main"]);
        Assert.Equal("content2", editorValue.Data["sidebar"]);
    }

    [Fact]
    public void EditorValue_Construct_FromNullDictionary_ShouldStoreEmptyDictionary()
    {
        // Act
        var editorValue = new EditorValue((Dictionary<string, string>?)null);

        // Assert
        Assert.Empty(editorValue.Data);
    }

    [Fact]
    public void EditorValue_ImplicitConversion_FromString_Works()
    {
        // Arrange
        var str = "content";

        // Act
        EditorValue editorValue = str;

        // Assert
        Assert.Single(editorValue.Data);
        Assert.Equal(str, editorValue.Data["main"]);
    }

    [Fact]
    public void EditorValue_ImplicitConversion_FromDictionary_Works()
    {
        // Arrange
        var dict = new Dictionary<string, string> { { "head", "val" } };

        // Act
        EditorValue editorValue = dict;

        // Assert
        Assert.Single(editorValue.Data);
        Assert.Equal("val", editorValue.Data["head"]);
    }

    [Fact]
    public void EditorValue_ImplicitConversion_FromNullDictionary_Works()
    {
        // Act
        EditorValue editorValue = (Dictionary<string, string>?)null;

        // Assert
        Assert.Empty(editorValue.Data);
    }
}
