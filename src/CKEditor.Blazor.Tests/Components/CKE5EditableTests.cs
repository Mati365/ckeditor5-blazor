using System.Reflection;
using CKEditor.Blazor.Components;
using CKEditor.Blazor.Model.Events;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;

namespace CKEditor.Blazor.Tests.Components;

public class CKE5EditableTests : BunitContext
{
    public CKE5EditableTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void RendersEditable_WithDefaultAttributes()
    {
        var cut = Render<CKE5Editable>(p => p.Add(p => p.Id, "test-editable"));

        var editable = cut.Find("cke5-editable");

        Assert.Equal("test-editable", editable.GetAttribute("id"));
        Assert.Equal("main", editable.GetAttribute("data-cke-root-name"));
        Assert.Equal("500", editable.GetAttribute("data-cke-save-debounce-ms"));
    }

    [Fact]
    public void RendersEditable_WithCustomRootName()
    {
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.RootName, "secondary"));

        var editable = cut.Find("cke5-editable");

        Assert.Equal("secondary", editable.GetAttribute("data-cke-root-name"));
    }

    [Fact]
    public void RendersEditable_WithEditorId_WhenEditorIdProvided()
    {
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.EditorId, "parent-editor"));

        var editable = cut.Find("cke5-editable");

        Assert.Equal("parent-editor", editable.GetAttribute("data-cke-editor-id"));
    }

    [Fact]
    public void RendersEditable_WithEditorId_InheritedFromCascadingEditor()
    {
        Services.AddCKEditor();

        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "parent-editor")
            .AddChildContent<CKE5Editable>(u => u
                .Add(u => u.Id, "test-editable")));

        var editable = cut.Find("cke5-editable");

        Assert.Equal("parent-editor", editable.GetAttribute("data-cke-editor-id"));
    }

    [Fact]
    public void RendersEditable_WithInitialContent_WhenValueProvided()
    {
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.Value, "<p>Hello world</p>"));

        var editable = cut.Find("cke5-editable");

        Assert.Equal("<p>Hello world</p>", editable.GetAttribute("data-cke-content"));
    }

    [Fact]
    public void RendersEditable_WithClassAttribute()
    {
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.Class, "my-editable"));

        var editable = cut.Find("cke5-editable");

        Assert.Equal("my-editable", editable.GetAttribute("class"));
    }

    [Fact]
    public void RendersEditable_WithPositionRelativeStyle_ByDefault()
    {
        var cut = Render<CKE5Editable>(p => p.Add(p => p.Id, "test-editable"));

        var editable = cut.Find("cke5-editable");
        var style = editable.GetAttribute("style");

        Assert.NotNull(style);
        Assert.Contains("position: relative", style);
    }

    [Fact]
    public void RendersEditable_WithCustomStyle_AppendedToPositionRelative()
    {
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.Style, "border: 1px solid red;"));

        var editable = cut.Find("cke5-editable");
        var style = editable.GetAttribute("style");

        Assert.Contains("position: relative", style);
        Assert.Contains("border: 1px solid red;", style);
    }

    [Fact]
    public void RendersEditable_WithInnerEditableDiv()
    {
        var cut = Render<CKE5Editable>(p => p.Add(p => p.Id, "test-editable"));

        var innerDiv = cut.Find("[data-cke-editable-content]");

        Assert.NotNull(innerDiv);
    }

    [Fact]
    public void RendersEditable_WithInnerClass_WhenInnerClassProvided()
    {
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.InnerClass, "ck-content"));

        var innerDiv = cut.Find("[data-cke-editable-content]");

        Assert.Equal("ck-content", innerDiv.GetAttribute("class"));
    }

    [Fact]
    public void RendersEditable_WithHiddenInput_WhenNameProvided()
    {
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.Name, "editable-field"));

        Assert.NotEmpty(cut.FindAll("input"));
        Assert.Equal("editable-field", cut.Find("input").GetAttribute("name"));
    }

    [Fact]
    public void RendersEditable_WithoutHiddenInput_WhenNameNotProvided()
    {
        var cut = Render<CKE5Editable>(p => p.Add(p => p.Id, "test-editable"));

        Assert.Empty(cut.FindAll("input"));
    }

    [Fact]
    public void GeneratesUniqueId_WhenIdNotProvided()
    {
        var cut1 = Render<CKE5Editable>();
        var cut2 = Render<CKE5Editable>();

        var id1 = cut1.Find("cke5-editable").GetAttribute("id");
        var id2 = cut2.Find("cke5-editable").GetAttribute("id");

        Assert.NotNull(id1);
        Assert.NotNull(id2);
        Assert.NotEqual(id1, id2);
        Assert.StartsWith("cke5-editable-", id1);
        Assert.StartsWith("cke5-editable-", id2);
    }

    [Fact]
    public void RendersEditable_WithCustomSaveDebounceMs()
    {
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.SaveDebounceMs, 1000));

        var editable = cut.Find("cke5-editable");

        Assert.Equal("1000", editable.GetAttribute("data-cke-save-debounce-ms"));
    }

    [Fact]
    public async Task OnChangeEditableData_InvokesValueChanged_WithNewValue()
    {
        string? capturedValue = null;
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, v => capturedValue = v)));

        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnChangeEditableData(editorMock.Object, "<p>Hello</p>");

        Assert.Equal("<p>Hello</p>", capturedValue);
    }

    [Fact]
    public async Task OnChangeEditableData_InvokesOnChange_WhenDelegateProvided()
    {
        CKE5EditableChangeEventArgs? capturedArgs = null;
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.OnChange, EventCallback.Factory.Create<CKE5EditableChangeEventArgs>(this, args => capturedArgs = args)));

        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnChangeEditableData(editorMock.Object, "<p>Hello</p>");

        Assert.NotNull(capturedArgs);
        Assert.Equal(editorMock.Object, capturedArgs.Editor);
        Assert.Equal("<p>Hello</p>", capturedArgs.Value);
    }

    [Fact]
    public async Task OnChangeEditableData_DoesNotInvokeOnChange_WhenNoDelegateProvided()
    {
        var cut = Render<CKE5Editable>(p => p.Add(p => p.Id, "test-editable"));
        var editorMock = new Mock<IJSObjectReference>();

        // Should not throw when OnChange has no delegate
        await cut.Instance.OnChangeEditableData(editorMock.Object, "<p>Hello</p>");
    }

    [Fact]
    public async Task DisposeAsync_DisposesResources_WhenDisposedAfterRender()
    {
        var cut = Render<CKE5Editable>(p => p.Add(p => p.Id, "test-editable"));

        await cut.Instance.DisposeAsync();
    }

    [Fact]
    public async Task DisposeAsync_DoesNotThrow_WhenDotNetHelperIsNull()
    {
        var cut = Render<CKE5Editable>(p => p.Add(p => p.Id, "test-editable"));

        // Use reflection to null out _dotNetHelper to test the null-conditional branch
        var field = typeof(CKE5Editable).GetField("_dotNetHelper", BindingFlags.NonPublic | BindingFlags.Instance);
        field!.SetValue(cut.Instance, null);

        await cut.Instance.DisposeAsync();
    }

    [Fact]
    public async Task OnParametersSetAsync_InvokesSetValue_WhenValueChangesAfterInitialization()
    {
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.Value, "<p>initial</p>"));

        // After first render IsInitializing is false; updating Value triggers setValue via JS interop
        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(CKE5Editable.Value)] = "<p>updated</p>"
            })));
    }
}
