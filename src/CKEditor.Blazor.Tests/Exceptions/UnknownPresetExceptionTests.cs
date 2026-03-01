using CKEditor.Blazor.Exceptions;

namespace CKEditor.Blazor.Tests.Exceptions;

public class UnknownPresetExceptionTests
{
    [Fact]
    public void Constructor_WithPresetName_ShouldSetFormattedMessage()
    {
        var presetName = "MyTestPreset";
        var exception = new UnknownPresetException(presetName);

        Assert.Equal($"Unknown preset: {presetName}", exception.Message);
        Assert.Null(exception.InnerException);
        Assert.IsAssignableFrom<CKEditorException>(exception);
    }
}
