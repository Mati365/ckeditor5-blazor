using CKEditor.Blazor.Exceptions;

namespace CKEditor.Blazor.Tests.Exceptions;
public class CloudConfigurationMissingExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultMessage()
    {
        var exception = new CloudConfigurationMissingException();

        Assert.Equal("Cloud configuration is missing for used preset.", exception.Message);
        Assert.Null(exception.InnerException);
        Assert.IsAssignableFrom<CloudConfigurationException>(exception);
    }
}
