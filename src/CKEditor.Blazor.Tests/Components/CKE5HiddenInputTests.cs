using CKEditor.Blazor.Components;

namespace CKEditor.Blazor.Tests.Components;

public class CKE5HiddenInputTests : BunitContext
{
    [Fact]
    public void RendersInput_WithDefaultStyles_WhenNoStyleProvided()
    {
        var cut = Render<CKE5HiddenInput>(p => p
            .Add(p => p.Id, "test-id")
            .Add(p => p.Name, "test-name"));

        var input = cut.Find("input");

        Assert.Contains("position: absolute", input.GetAttribute("style"));
        Assert.Contains("opacity: 0", input.GetAttribute("style"));
        Assert.Contains("pointer-events: none", input.GetAttribute("style"));
    }

    [Fact]
    public void RendersInput_WithCustomStyle_WhenStyleProvided()
    {
        var cut = Render<CKE5HiddenInput>(p => p
            .Add(p => p.Id, "test-id")
            .Add(p => p.Style, "display: none;"));

        var input = cut.Find("input");

        Assert.Equal("display: none;", input.GetAttribute("style"));
    }

    [Fact]
    public void RendersInput_WithNameAttribute()
    {
        var cut = Render<CKE5HiddenInput>(p => p
            .Add(p => p.Id, "test-id")
            .Add(p => p.Name, "my-field"));

        var input = cut.Find("input");

        Assert.Equal("my-field", input.GetAttribute("name"));
    }

    [Fact]
    public void RendersInput_WithIdAttribute()
    {
        var cut = Render<CKE5HiddenInput>(p => p
            .Add(p => p.Id, "custom-id"));

        var input = cut.Find("input");

        Assert.Equal("custom-id", input.GetAttribute("id"));
    }

    [Fact]
    public void RendersInput_WithRequiredAttribute_WhenRequiredIsTrue()
    {
        var cut = Render<CKE5HiddenInput>(p => p
            .Add(p => p.Id, "test-id")
            .Add(p => p.Required, true));

        var input = cut.Find("input");

        Assert.NotNull(input.GetAttribute("required"));
    }

    [Fact]
    public void RendersInput_WithoutRequiredAttribute_WhenRequiredIsFalse()
    {
        var cut = Render<CKE5HiddenInput>(p => p
            .Add(p => p.Id, "test-id")
            .Add(p => p.Required, false));

        var input = cut.Find("input");

        Assert.Null(input.GetAttribute("required"));
    }

    [Fact]
    public void RendersInput_WithClassAttribute()
    {
        var cut = Render<CKE5HiddenInput>(p => p
            .Add(p => p.Id, "test-id")
            .Add(p => p.Class, "my-class"));

        var input = cut.Find("input");

        Assert.Equal("my-class", input.GetAttribute("class"));
    }

    [Fact]
    public void GeneratesUniqueId_WhenIdNotProvided()
    {
        var cut1 = Render<CKE5HiddenInput>();
        var cut2 = Render<CKE5HiddenInput>();

        var id1 = cut1.Find("input").GetAttribute("id");
        var id2 = cut2.Find("input").GetAttribute("id");

        Assert.NotNull(id1);
        Assert.NotNull(id2);
        Assert.NotEqual(id1, id2);
        Assert.StartsWith("cke5-input-", id1);
        Assert.StartsWith("cke5-input-", id2);
    }
}
