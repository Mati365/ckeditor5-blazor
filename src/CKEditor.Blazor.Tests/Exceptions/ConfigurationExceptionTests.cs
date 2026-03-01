
using CKEditor.Blazor.Exceptions;

namespace CKEditor.Blazor.Tests.Exceptions;
public class ConfigurationExceptionTests
{
    [Fact]
    public void Constructor_ShouldCreateExceptionWithoutMessage()
    {
        var exception = new ConfigurationException();

        Assert.Null(exception.InnerException);
        Assert.NotNull(exception.Message);
        Assert.IsAssignableFrom<CKEditorException>(exception);
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessageProperty()
    {
        var message = "Test exception message";
        var exception = new ConfigurationException(message);

        Assert.Equal(message, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetProperties()
    {
        var message = "Test exception message";
        var innerException = new Exception("Inner exception");
        var exception = new ConfigurationException(message, innerException);

        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }
}
