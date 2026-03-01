using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Tests.Model;

public class PresetElementSelectorTests
{
    [Fact]
    public void PresetElementSelector_ShouldSetSelectorProperty()
    {
        var selectorValue = ".my-class";

        var elementSelector = new PresetElementSelector(selectorValue);

        Assert.Equal(selectorValue, elementSelector.Selector);
    }
}
