using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Tests.Model;

public class EditorTypeTests
{
    [Fact]
    public void EditorType_ShouldHaveExpectedValues()
    {
        // Act & Assert
        Assert.Equal(0, (int)EditorType.Classic);
        Assert.Equal(1, (int)EditorType.Inline);
        Assert.Equal(2, (int)EditorType.Balloon);
        Assert.Equal(3, (int)EditorType.Decoupled);
        Assert.Equal(4, (int)EditorType.Multiroot);
    }
}
