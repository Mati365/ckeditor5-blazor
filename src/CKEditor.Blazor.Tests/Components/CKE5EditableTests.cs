using System.Reflection;
using CKEditor.Blazor.Components;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Events;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;

namespace CKEditor.Blazor.Tests.Components;

public class CKE5EditableTests : BunitContext
{
    public CKE5EditableTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    // -------------------------------------------------------------------------
    // Initial render
    // -------------------------------------------------------------------------

    [Fact]
    public void RendersEditable_WithDefaultAttributes()
    {
        var cut = Render<CKE5Editable>(static p => p.Add(static p => p.Id, "test-editable"));

        var editable = cut.Find("cke5-editable");

        Assert.Equal("test-editable", editable.GetAttribute("id"));
        Assert.Equal("main", editable.GetAttribute("data-cke-root-name"));
        Assert.Equal("500", editable.GetAttribute("data-cke-save-debounce-ms"));
    }

    [Fact]
    public void RendersEditable_WithRootAttributes_WhenProvided()
    {
        var rootAttributes = new EditorRootAttributes
        {
            ["data-test"] = "value",
            ["aria-label"] = "Editable root"
        };

        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.RootAttributes, rootAttributes));

        var rootAttrsJson = cut.Find("cke5-editable").GetAttribute("data-cke-root-attributes");

        Assert.NotNull(rootAttrsJson);
        Assert.Contains("\"data-test\":\"value\"", rootAttrsJson);
        Assert.Contains("\"aria-label\":\"Editable root\"", rootAttrsJson);
    }

    [Fact]
    public void RendersEditable_WithoutRootAttributesAttr_WhenRootAttributesIsEmpty()
    {
        // An empty EditorRootAttributes (Count == 0) should not emit the attribute at all.
        var cut = Render<CKE5Editable>(static p => p
            .Add(static p => p.Id, "test-editable")
            .Add(static p => p.RootAttributes, new EditorRootAttributes()));

        Assert.Null(cut.Find("cke5-editable").GetAttribute("data-cke-root-attributes"));
    }

    [Fact]
    public void RendersEditable_WithCustomRootName()
    {
        var cut = Render<CKE5Editable>(static p => p
            .Add(static p => p.Id, "test-editable")
            .Add(static p => p.RootName, "secondary"));

        Assert.Equal("secondary", cut.Find("cke5-editable").GetAttribute("data-cke-root-name"));
    }

    [Fact]
    public void RendersEditable_WithEditorId_WhenEditorIdProvided()
    {
        var cut = Render<CKE5Editable>(static p => p
            .Add(static p => p.Id, "test-editable")
            .Add(static p => p.EditorId, "parent-editor"));

        Assert.Equal("parent-editor", cut.Find("cke5-editable").GetAttribute("data-cke-editor-id"));
    }

    [Fact]
    public void RendersEditable_WithEditorId_InheritedFromCascadingEditor()
    {
        Services.AddCKEditor();

        var cut = Render<CKE5Editor>(static p => p
            .Add(static p => p.Id, "parent-editor")
            .AddChildContent<CKE5Editable>(static u => u
                .Add(static u => u.Id, "test-editable")));

        Assert.Equal("parent-editor", cut.Find("cke5-editable").GetAttribute("data-cke-editor-id"));
    }

    [Fact]
    public void RendersEditable_WithInitialContent_WhenValueProvided()
    {
        var cut = Render<CKE5Editable>(static p => p
            .Add(static p => p.Id, "test-editable")
            .Add(static p => p.Value, "<p>Hello world</p>"));

        Assert.Equal("<p>Hello world</p>", cut.Find("cke5-editable").GetAttribute("data-cke-content"));
    }

    [Fact]
    public void RendersEditable_WithClassAttribute()
    {
        var cut = Render<CKE5Editable>(static p => p
            .Add(static p => p.Id, "test-editable")
            .Add(static p => p.Class, "my-editable"));

        Assert.Equal("my-editable", cut.Find("cke5-editable").GetAttribute("class"));
    }

    [Fact]
    public void RendersEditable_WithPositionRelativeStyle_ByDefault()
    {
        var cut = Render<CKE5Editable>(static p => p.Add(static p => p.Id, "test-editable"));

        var style = cut.Find("cke5-editable").GetAttribute("style");

        Assert.NotNull(style);
        Assert.Contains("position: relative", style);
    }

    [Fact]
    public void RendersEditable_WithCustomStyle_AppendedToPositionRelative()
    {
        var cut = Render<CKE5Editable>(static p => p
            .Add(static p => p.Id, "test-editable")
            .Add(static p => p.Style, "border: 1px solid red;"));

        var style = cut.Find("cke5-editable").GetAttribute("style");

        Assert.Contains("position: relative", style);
        Assert.Contains("border: 1px solid red;", style);
    }

    [Fact]
    public void RendersEditable_WithInnerEditableDiv()
    {
        var cut = Render<CKE5Editable>(static p => p.Add(static p => p.Id, "test-editable"));

        Assert.NotNull(cut.Find("[data-cke-editable-content]"));
    }

    [Fact]
    public void RendersEditable_WithInnerClass_WhenInnerClassProvided()
    {
        var cut = Render<CKE5Editable>(static p => p
            .Add(static p => p.Id, "test-editable")
            .Add(static p => p.InnerClass, "ck-content"));

        Assert.Equal("ck-content", cut.Find("[data-cke-editable-content]").GetAttribute("class"));
    }

    [Fact]
    public void RendersEditable_WithHiddenInput_WhenNameProvided()
    {
        var cut = Render<CKE5Editable>(static p => p
            .Add(static p => p.Id, "test-editable")
            .Add(static p => p.Name, "editable-field"));

        var input = cut.Find("input");

        Assert.Equal("editable-field", input.GetAttribute("name"));
    }

    [Fact]
    public void RendersEditable_WithoutHiddenInput_WhenNameNotProvided()
    {
        var cut = Render<CKE5Editable>(static p => p.Add(static p => p.Id, "test-editable"));

        Assert.Empty(cut.FindAll("input"));
    }

    [Fact]
    public void RendersEditable_WithCustomSaveDebounceMs()
    {
        var cut = Render<CKE5Editable>(static p => p
            .Add(static p => p.Id, "test-editable")
            .Add(static p => p.SaveDebounceMs, 1000));

        Assert.Equal("1000", cut.Find("cke5-editable").GetAttribute("data-cke-save-debounce-ms"));
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
    public async Task OnParametersSetAsync_UpdatesRenderedContent_WhenValueChangesAfterInitialization()
    {
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.Value, "<p>initial</p>"));

        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(CKE5Editable.Value)] = "<p>updated</p>"
            })));

        // Both the component property and the rendered data attribute must reflect the new value.
        Assert.Equal("<p>updated</p>", cut.Instance.Value);
        Assert.Equal("<p>updated</p>", cut.Find("cke5-editable").GetAttribute("data-cke-content"));
    }

    [Fact]
    public async Task OnParametersSetAsync_UpdatesRenderedRootAttributesJson_WhenRootAttributesChangesAfterInitialization()
    {
        var initial = new EditorRootAttributes { ["data-section"] = "intro" };

        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.RootAttributes, initial));

        var updated = new EditorRootAttributes
        {
            ["data-section"] = "outro",
            ["aria-label"] = "Updated root"
        };

        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(CKE5Editable.RootAttributes)] = updated
            })));

        var rootAttrsJson = cut.Find("cke5-editable").GetAttribute("data-cke-root-attributes");

        Assert.NotNull(rootAttrsJson);
        Assert.Contains("\"data-section\":\"outro\"", rootAttrsJson);
        Assert.Contains("\"aria-label\":\"Updated root\"", rootAttrsJson);
        Assert.DoesNotContain("intro", rootAttrsJson);
    }

    [Fact]
    public async Task OnParametersSetAsync_RemovesRootAttributesAttr_WhenRootAttributesBecomesNull()
    {
        var initial = new EditorRootAttributes { ["data-section"] = "intro" };

        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.RootAttributes, initial));

        Assert.NotNull(cut.Find("cke5-editable").GetAttribute("data-cke-root-attributes"));

        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(CKE5Editable.RootAttributes)] = null
            })));

        Assert.Null(cut.Find("cke5-editable").GetAttribute("data-cke-root-attributes"));
    }

    [Fact]
    public async Task OnParametersSetAsync_UpdatesBothContentAndRootAttributes_WhenBothChangeSimultaneously()
    {
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.Value, "<p>old</p>")
            .Add(p => p.RootAttributes, new EditorRootAttributes { ["data-x"] = "1" }));

        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(CKE5Editable.Value)] = "<p>new</p>",
                [nameof(CKE5Editable.RootAttributes)] = new EditorRootAttributes { ["data-x"] = "2" }
            })));

        var editable = cut.Find("cke5-editable");

        Assert.Equal("<p>new</p>", editable.GetAttribute("data-cke-content"));
        Assert.Contains("\"data-x\":\"2\"", editable.GetAttribute("data-cke-root-attributes"));
    }

    [Fact]
    public async Task OnChangeEditableData_UpdatesValueProperty_AndInvokesValueChanged()
    {
        string? capturedValue = null;
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, v => capturedValue = v)));

        await cut.Instance.OnChangeEditableData(new Mock<IJSObjectReference>().Object, "<p>Hello</p>");

        Assert.Equal("<p>Hello</p>", cut.Instance.Value);
        Assert.Equal("<p>Hello</p>", capturedValue);
    }

    [Fact]
    public async Task OnChangeEditableData_InvokesOnChange_WithCorrectArgs_WhenDelegateProvided()
    {
        CKE5EditableChangeEventArgs? capturedArgs = null;
        var cut = Render<CKE5Editable>(p => p
            .Add(p => p.Id, "test-editable")
            .Add(p => p.RootName, "secondary")
            .Add(p => p.OnChange, EventCallback.Factory.Create<CKE5EditableChangeEventArgs>(
                this, args => capturedArgs = args)));

        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnChangeEditableData(editorMock.Object, "<p>Hello</p>");

        Assert.NotNull(capturedArgs);
        Assert.Equal(editorMock.Object, capturedArgs.Editor);
        Assert.Equal("<p>Hello</p>", capturedArgs.Value);
        Assert.Equal("secondary", capturedArgs.RootName);
    }

    [Fact]
    public async Task OnChangeEditableData_StillUpdatesValue_WhenOnChangeHasNoDelegate()
    {
        var cut = Render<CKE5Editable>(static p => p.Add(static p => p.Id, "test-editable"));

        await cut.Instance.OnChangeEditableData(new Mock<IJSObjectReference>().Object, "<p>Hello</p>");

        // OnChange not configured — the component must still track the new value.
        Assert.Equal("<p>Hello</p>", cut.Instance.Value);
    }

    [Fact]
    public async Task DisposeAsync_CanBeCalledMultipleTimes_WithoutThrowing()
    {
        var cut = Render<CKE5Editable>(static p => p.Add(static p => p.Id, "test-editable"));

        await cut.Instance.DisposeAsync();
        await cut.Instance.DisposeAsync(); // second call must also be safe
    }

    [Fact]
    public async Task DisposeAsync_DoesNotThrow_WhenDotNetHelperIsNull()
    {
        var cut = Render<CKE5Editable>(p => p.Add(p => p.Id, "test-editable"));

        var field = typeof(CKE5Editable)
            .GetField("_dotNetHelper", BindingFlags.NonPublic | BindingFlags.Instance);

        field!.SetValue(cut.Instance, null);

        // The null-conditional ?. in Dispose must not throw.
        var exception = await Record.ExceptionAsync(() => cut.Instance.DisposeAsync().AsTask());

        Assert.Null(exception);
    }
}
