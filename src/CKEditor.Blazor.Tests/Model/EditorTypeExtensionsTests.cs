using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Tests.Model;

public class EditorTypeExtensionsTests
{
    [Theory]
    [InlineData(EditorType.Decoupled, true)]
    [InlineData(EditorType.Multiroot, true)]
    [InlineData(EditorType.Classic, false)]
    [InlineData(EditorType.Inline, false)]
    [InlineData(EditorType.Balloon, false)]
    public void IsDecoupledOrMultiroot_ShouldReturnExpectedResult(EditorType editorType, bool expected)
    {
        var result = editorType.IsDecoupledOrMultiroot();

        Assert.Equal(expected, result);
    }
}
