using CKEditor.Blazor.Exceptions;

namespace CKEditor.Blazor.Tests.Exceptions;

public class SelfHostedLicenseIncompatibleExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessageProperty()
    {
        var message = "Test exception message";
        var exception = new SelfHostedLicenseIncompatibleException(message);

        Assert.Equal(message, exception.Message);
        Assert.Null(exception.InnerException);
        Assert.IsAssignableFrom<CKEditorException>(exception);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetProperties()
    {
        var message = "Test exception message";
        var innerException = new Exception("Inner exception");
        var exception = new SelfHostedLicenseIncompatibleException(message, innerException);

        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }
}
