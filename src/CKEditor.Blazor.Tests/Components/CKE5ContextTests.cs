using CKEditor.Blazor.Components;
using CKEditor.Blazor.Services;

namespace CKEditor.Blazor.Tests.Components;

public class CKE5ContextTests : BunitContext
{
    public CKE5ContextTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddCKEditor();
    }

    [Fact]
    public void RendersContext_WithDefaultAttributes()
    {
        var cut = Render<CKE5Context>(static p => p.Add(static p => p.Id, "test-context"));

        var context = cut.Find("cke5-context");

        Assert.Equal("test-context", context.GetAttribute("data-cke-context-id"));
        Assert.Null(context.GetAttribute("data-cke-language"));
        Assert.NotNull(context.GetAttribute("data-cke-context"));
    }

    [Fact]
    public void RendersContext_WithCustomId()
    {
        var cut = Render<CKE5Context>(static p => p.Add(static p => p.Id, "my-context-id"));

        var context = cut.Find("cke5-context");

        Assert.Equal("my-context-id", context.GetAttribute("data-cke-context-id"));
    }

    [Fact]
    public void GeneratesUniqueId_WhenIdNotProvided()
    {
        var cut1 = Render<CKE5Context>();
        var cut2 = Render<CKE5Context>();

        var id1 = cut1.Find("cke5-context").GetAttribute("data-cke-context-id");
        var id2 = cut2.Find("cke5-context").GetAttribute("data-cke-context-id");

        Assert.NotNull(id1);
        Assert.NotNull(id2);
        Assert.NotEqual(id1, id2);
        Assert.StartsWith("cke5-context-", id1);
        Assert.StartsWith("cke5-context-", id2);
    }

    [Fact]
    public void RendersContext_WithLanguageJson_WhenLanguageProvided()
    {
        var cut = Render<CKE5Context>(static p => p
            .Add(static p => p.Id, "test-context")
            .Add(static p => p.Language, "pl"));

        var context = cut.Find("cke5-context");
        var languageJson = context.GetAttribute("data-cke-language");

        Assert.NotNull(languageJson);
        Assert.Contains("pl", languageJson);
    }

    [Fact]
    public void RendersContext_WithChildContent()
    {
        var cut = Render<CKE5Context>(static p => p
            .Add(static p => p.Id, "test-context")
            .AddChildContent("<div class=\"child\">Editor here</div>"));

        Assert.NotEmpty(cut.FindAll("div.child"));
    }

    [Fact]
    public void RendersContext_AsCustomHtmlElement()
    {
        var cut = Render<CKE5Context>(static p => p.Add(static p => p.Id, "test-context"));

        Assert.Single(cut.FindAll("cke5-context"));
    }
}
