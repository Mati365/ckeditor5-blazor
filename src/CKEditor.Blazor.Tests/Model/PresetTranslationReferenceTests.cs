using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Tests.Model;

public class PresetTranslationReferenceTests
{
    [Fact]
    public void PresetTranslationReference_ShouldSetKeyProperty()
    {
        // Arrange
        var translationKey = "Save";

        // Act
        var translationReference = new PresetTranslationReference(translationKey);

        // Assert
        Assert.Equal(translationKey, translationReference.Key);
    }
}
