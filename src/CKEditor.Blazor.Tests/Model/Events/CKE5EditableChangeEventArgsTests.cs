using CKEditor.Blazor.Model.Events;
using Microsoft.JSInterop;
using Moq;

namespace CKEditor.Blazor.Tests.Model.Events;

public class CKE5EditableChangeEventArgsTests
{
    [Fact]
    public void CKE5EditableChangeEventArgs_ShouldSetProperties()
    {
        var rootName = "main";
        var editorMock = new Mock<IJSObjectReference>();
        var value = "new value";

        var args = new CKE5EditableChangeEventArgs(rootName, editorMock.Object, value);

        Assert.Equal(rootName, args.RootName);
        Assert.Same(editorMock.Object, args.Editor);
        Assert.Equal(value, args.Value);
    }
}
