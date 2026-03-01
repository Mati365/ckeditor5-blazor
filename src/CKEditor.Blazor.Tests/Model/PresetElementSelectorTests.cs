using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Tests.Model;

public class PresetElementSelectorTests
{
    [Fact]
    public void PresetElementSelector_ShouldSetSelectorProperty()
    {
        // Arrange
        var selectorValue = ".my-class";

        // Act
        var elementSelector = new PresetElementSelector(selectorValue);

        // Assert
        Assert.Equal(selectorValue, elementSelector.Selector);
    }
}
