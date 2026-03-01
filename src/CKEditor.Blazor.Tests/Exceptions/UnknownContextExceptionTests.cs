using CKEditor.Blazor.Exceptions;

namespace CKEditor.Blazor.Tests.Exceptions;

public class UnknownContextExceptionTests
{
    [Fact]
    public void Constructor_WithContextName_ShouldSetFormattedMessage()
    {
        var contextName = "MyTestContext";
        var exception = new UnknownContextException(contextName);

        Assert.Equal($"Unknown context: {contextName}", exception.Message);
        Assert.Null(exception.InnerException);
        Assert.IsAssignableFrom<CKEditorException>(exception);
    }
}
