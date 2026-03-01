using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Tests.Model;

public class PresetTranslationReferenceTests
{
    [Fact]
    public void PresetTranslationReference_ShouldSetKeyProperty()
    {
        var translationKey = "Save";

        var translationReference = new PresetTranslationReference(translationKey);

        Assert.Equal(translationKey, translationReference.Key);
    }
}
