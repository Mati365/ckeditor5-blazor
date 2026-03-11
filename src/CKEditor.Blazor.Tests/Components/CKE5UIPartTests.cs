using System.Reflection;
using CKEditor.Blazor.Components;
using CKEditor.Blazor.Services;

namespace CKEditor.Blazor.Tests.Components;

public class CKE5UIPartTests : BunitContext
{
    public CKE5UIPartTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void RendersUIPart_WithNameAttribute()
    {
        var cut = Render<CKE5UIPart>(p => p
            .Add(p => p.Id, "test-ui-part")
            .Add(p => p.Name, "toolbar"));

        var uiPart = cut.Find("cke5-ui-part");

        Assert.Equal("toolbar", uiPart.GetAttribute("data-cke-name"));
    }

    [Fact]
    public void RendersUIPart_WithIdAttribute()
    {
        var cut = Render<CKE5UIPart>(p => p.Add(p => p.Id, "my-ui-part"));

        var uiPart = cut.Find("cke5-ui-part");

        Assert.Equal("my-ui-part", uiPart.GetAttribute("id"));
    }

    [Fact]
    public void RendersUIPart_WithEditorId_WhenEditorIdProvided()
    {
        var cut = Render<CKE5UIPart>(p => p
            .Add(p => p.Id, "test-ui-part")
            .Add(p => p.EditorId, "parent-editor"));

        var uiPart = cut.Find("cke5-ui-part");

        Assert.Equal("parent-editor", uiPart.GetAttribute("data-cke-editor-id"));
    }

    [Fact]
    public void RendersUIPart_WithEditorId_InheritedFromCascadingEditor()
    {
        Services.AddCKEditor();

        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "parent-editor")
            .AddChildContent<CKE5UIPart>(u => u
                .Add(u => u.Id, "test-ui-part")));

        var uiPart = cut.Find("cke5-ui-part");

        Assert.Equal("parent-editor", uiPart.GetAttribute("data-cke-editor-id"));
    }

    [Fact]
    public void RendersUIPart_WithClassAttribute()
    {
        var cut = Render<CKE5UIPart>(p => p
            .Add(p => p.Id, "test-ui-part")
            .Add(p => p.Name, "toolbar")
            .Add(p => p.Class, "toolbar-container"));

        var uiPart = cut.Find("cke5-ui-part");

        Assert.Equal("toolbar-container", uiPart.GetAttribute("class"));
    }

    [Fact]
    public void RendersUIPart_WithStyleAttribute()
    {
        var cut = Render<CKE5UIPart>(p => p
            .Add(p => p.Id, "test-ui-part")
            .Add(p => p.Name, "toolbar")
            .Add(p => p.Style, "border: 1px solid blue;"));

        var uiPart = cut.Find("cke5-ui-part");

        Assert.Equal("border: 1px solid blue;", uiPart.GetAttribute("style"));
    }

    [Fact]
    public void RendersUIPart_WithInteractiveAttribute_WhenInteractiveIsTrue()
    {
        var cut = Render<CKE5UIPart>(p => p
            .Add(p => p.Id, "test-ui-part")
            .Add(p => p.Name, "toolbar")
            .Add(p => p.Interactive, true));

        var uiPart = cut.Find("cke5-ui-part");

        Assert.NotNull(uiPart.GetAttribute("data-cke-interactive"));
    }

    [Fact]
    public void GeneratesUniqueId_WhenIdNotProvided()
    {
        var cut1 = Render<CKE5UIPart>(p => p.Add(p => p.Name, "toolbar"));
        var cut2 = Render<CKE5UIPart>(p => p.Add(p => p.Name, "toolbar"));

        var id1 = cut1.Find("cke5-ui-part").GetAttribute("id");
        var id2 = cut2.Find("cke5-ui-part").GetAttribute("id");

        Assert.NotNull(id1);
        Assert.NotNull(id2);
        Assert.NotEqual(id1, id2);
        Assert.StartsWith("cke5-ui-part-", id1);
        Assert.StartsWith("cke5-ui-part-", id2);
    }

    [Fact]
    public void RendersUIPart_AsCustomHtmlElement()
    {
        var cut = Render<CKE5UIPart>(p => p
            .Add(p => p.Id, "test-ui-part")
            .Add(p => p.Name, "menubar"));

        Assert.Single(cut.FindAll("cke5-ui-part"));
    }

    [Fact]
    public async Task DisposeAsync_DisposesResources_WhenDisposedAfterRender()
    {
        var cut = Render<CKE5UIPart>(p => p.Add(p => p.Id, "test-ui-part"));

        await cut.Instance.DisposeAsync();
    }

    [Fact]
    public async Task DisposeAsync_DoesNotThrow_WhenDotNetHelperIsNull()
    {
        var cut = Render<CKE5UIPart>(p => p.Add(p => p.Id, "test-ui-part"));

        // Use reflection to null out _dotNetHelper to test the null-conditional branch
        var field = typeof(CKE5UIPart).GetField("_dotNetHelper", BindingFlags.NonPublic | BindingFlags.Instance);
        field!.SetValue(cut.Instance, null);

        await cut.Instance.DisposeAsync();
    }
}
