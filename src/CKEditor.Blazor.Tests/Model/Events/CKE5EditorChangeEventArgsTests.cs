using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Events;
using Microsoft.JSInterop;
using Moq;

namespace CKEditor.Blazor.Tests.Model.Events;

public class CKE5EditorChangeEventArgsTests
{
    [Fact]
    public void CKE5EditorChangeEventArgs_ShouldSetProperties()
    {
        var editorMock = new Mock<IJSObjectReference>();
        var value = new EditorValue("new value");

        var args = new CKE5EditorChangeEventArgs(editorMock.Object, value);

        Assert.Same(editorMock.Object, args.Editor);
        Assert.Same(value, args.Value);
    }
}
