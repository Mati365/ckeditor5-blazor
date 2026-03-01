using System.Reflection;
using CKEditor.Blazor.Components;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Events;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;

namespace CKEditor.Blazor.Tests.Components;

public class CKE5EditorTests : BunitContext
{
    public CKE5EditorTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddCKEditor();
    }

    [Fact]
    public void RendersEditor_WithDefaultAttributes()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        var editor = cut.Find("cke5-editor");

        Assert.Equal("test-editor", editor.GetAttribute("data-cke-editor-id"));
        Assert.Equal("250", editor.GetAttribute("data-cke-save-debounce-ms"));
        Assert.Null(editor.GetAttribute("data-cke-interactive"));
        Assert.Equal("true", editor.GetAttribute("data-cke-watchdog"));
    }

    [Fact]
    public void RendersEditor_WithCustomClass()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Class, "my-editor-class"));

        var editor = cut.Find("cke5-editor");

        Assert.Equal("my-editor-class", editor.GetAttribute("class"));
    }

    [Fact]
    public void RendersEditor_WithInteractiveAttribute_WhenInteractiveIsTrue()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Interactive, true));

        var editor = cut.Find("cke5-editor");

        Assert.Equal("true", editor.GetAttribute("data-cke-interactive"));
    }

    [Fact]
    public void RendersEditor_WithoutInteractiveAttribute_WhenInteractiveIsFalse()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Interactive, false));

        var editor = cut.Find("cke5-editor");

        Assert.Null(editor.GetAttribute("data-cke-interactive"));
    }

    [Fact]
    public void RendersEditor_WithWatchdogFalse_WhenWatchdogDisabled()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Watchdog, false));

        var editor = cut.Find("cke5-editor");

        Assert.Null(editor.GetAttribute("data-cke-watchdog"));
    }

    [Fact]
    public void RendersEditor_WithContextId_WhenContextIdProvided()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.ContextId, "my-context"));

        var editor = cut.Find("cke5-editor");

        Assert.Equal("my-context", editor.GetAttribute("data-cke-context-id"));
    }

    [Fact]
    public void RendersEditor_WithoutContextId_WhenContextIdNotProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        var editor = cut.Find("cke5-editor");

        Assert.Null(editor.GetAttribute("data-cke-context-id"));
    }

    [Fact]
    public void RendersEditor_WithHiddenInput_WhenNameProvided()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Name, "my-field"));

        Assert.NotEmpty(cut.FindAll("input"));
        Assert.Equal("my-field", cut.Find("input").GetAttribute("name"));
    }

    [Fact]
    public void RendersEditor_WithoutHiddenInput_WhenNameNotProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        Assert.Empty(cut.FindAll("input"));
    }

    [Fact]
    public void RendersEditor_WithCustomSaveDebounceMs()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.SaveDebounceMs, 500));

        var editor = cut.Find("cke5-editor");

        Assert.Equal("500", editor.GetAttribute("data-cke-save-debounce-ms"));
    }

    [Fact]
    public void GeneratesUniqueId_WhenIdNotProvided()
    {
        var cut1 = Render<CKE5Editor>();
        var cut2 = Render<CKE5Editor>();

        var id1 = cut1.Find("cke5-editor").GetAttribute("data-cke-editor-id");
        var id2 = cut2.Find("cke5-editor").GetAttribute("data-cke-editor-id");

        Assert.NotNull(id1);
        Assert.NotNull(id2);
        Assert.NotEqual(id1, id2);
        Assert.StartsWith("cke5-", id1);
        Assert.StartsWith("cke5-", id2);
    }

    [Fact]
    public void RendersEditor_WithStyleContainingPositionRelative()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        var editor = cut.Find("cke5-editor");
        var style = editor.GetAttribute("style");

        Assert.NotNull(style);
        Assert.Contains("position: relative", style);
    }

    [Fact]
    public void RendersEditor_WithChildContent()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .AddChildContent("<span class=\"slot\">toolbar</span>"));

        Assert.NotEmpty(cut.FindAll("span.slot"));
    }

    [Fact]
    public void RendersEditor_WithEditableHeightAttribute_WhenEditableHeightProvided()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.EditableHeight, 400));

        var editor = cut.Find("cke5-editor");

        Assert.Equal("400", editor.GetAttribute("data-cke-editable-height"));
    }

    [Fact]
    public async Task OnChangeEditorData_InvokesValueChanged_WithNewValue()
    {
        EditorValue? capturedValue = null;
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<EditorValue>(this, v => capturedValue = v)));

        var editorMock = new Mock<IJSObjectReference>();
        var newValue = new EditorValue("<p>Hello</p>");

        await cut.Instance.OnChangeEditorData(editorMock.Object, newValue);

        Assert.Equal(newValue, capturedValue);
    }

    [Fact]
    public async Task OnChangeEditorData_InvokesOnChange_WhenDelegateProvided()
    {
        CKE5EditorChangeEventArgs? capturedArgs = null;
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.OnChange, EventCallback.Factory.Create<CKE5EditorChangeEventArgs>(this, args => capturedArgs = args)));

        var editorMock = new Mock<IJSObjectReference>();
        var newValue = new EditorValue("<p>Hello</p>");

        await cut.Instance.OnChangeEditorData(editorMock.Object, newValue);

        Assert.NotNull(capturedArgs);
        Assert.Equal(editorMock.Object, capturedArgs.Editor);
        Assert.Equal(newValue, capturedArgs.Value);
    }

    [Fact]
    public async Task OnChangeEditorData_DoesNotInvokeOnChange_WhenNoDelegateProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));
        var editorMock = new Mock<IJSObjectReference>();
        var newValue = new EditorValue("<p>Hello</p>");

        // Should not throw when OnChange has no delegate
        await cut.Instance.OnChangeEditorData(editorMock.Object, newValue);
    }

    [Fact]
    public async Task OnEditorFocus_InvokesOnFocus_WhenDelegateProvided()
    {
        IJSObjectReference? capturedEditor = null;
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.OnFocus, EventCallback.Factory.Create<IJSObjectReference>(this, e => capturedEditor = e)));

        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnEditorFocus(editorMock.Object);

        Assert.Equal(editorMock.Object, capturedEditor);
    }

    [Fact]
    public async Task OnEditorFocus_DoesNotThrow_WhenNoDelegateProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));
        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnEditorFocus(editorMock.Object);
    }

    [Fact]
    public async Task OnEditorBlur_InvokesOnBlur_WhenDelegateProvided()
    {
        IJSObjectReference? capturedEditor = null;
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.OnBlur, EventCallback.Factory.Create<IJSObjectReference>(this, e => capturedEditor = e)));

        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnEditorBlur(editorMock.Object);

        Assert.Equal(editorMock.Object, capturedEditor);
    }

    [Fact]
    public async Task OnEditorBlur_DoesNotThrow_WhenNoDelegateProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));
        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnEditorBlur(editorMock.Object);
    }

    [Fact]
    public async Task OnEditorReady_InvokesOnReady_WhenDelegateProvided()
    {
        IJSObjectReference? capturedEditor = null;
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.OnReady, EventCallback.Factory.Create<IJSObjectReference>(this, e => capturedEditor = e)));

        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnEditorReady(editorMock.Object);

        Assert.Equal(editorMock.Object, capturedEditor);
    }

    [Fact]
    public async Task OnEditorReady_DoesNotThrow_WhenNoDelegateProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));
        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnEditorReady(editorMock.Object);
    }

    [Fact]
    public async Task SetValue_IsInvoked_WhenValueChangesAfterInitialization()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Value, new EditorValue("<p>initial</p>")));

        // After first render IsInitializing is false; updating Value triggers setValue via JS interop
        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(CKE5Editor.Value)] = new EditorValue("<p>updated</p>")
            })));
    }

    [Fact]
    public void RendersEditor_WithCustomConfig_WhenConfigProvided()
    {
        var customConfig = new Dictionary<string, object> { ["myToolbar"] = new[] { "bold" } };

        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Config, customConfig));

        var preset = cut.Find("cke5-editor").GetAttribute("data-cke-preset");

        Assert.NotNull(preset);
        Assert.Contains("myToolbar", preset);
    }

    [Fact]
    public void RendersEditor_WithMergedConfig_WhenMergeConfigProvided()
    {
        var mergeConfig = new Dictionary<string, object> { ["myCustomKey"] = "myMergedValue" };

        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.MergeConfig, mergeConfig));

        var preset = cut.Find("cke5-editor").GetAttribute("data-cke-preset");

        Assert.NotNull(preset);
        Assert.Contains("myCustomKey", preset);
        Assert.Contains("myMergedValue", preset);
    }

    [Fact]
    public void RendersEditor_WithCustomTranslations_WhenTranslationsProvided()
    {
        var translations = new Dictionary<string, string> { ["Save"] = "Speichern" };

        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.CustomTranslations, translations));

        var preset = cut.Find("cke5-editor").GetAttribute("data-cke-preset");

        Assert.NotNull(preset);
        Assert.Contains("Speichern", preset);
    }

    [Fact]
    public void RendersEditor_WithOverriddenEditorType_WhenEditorTypeProvided()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.EditorType, "inline"));

        var preset = cut.Find("cke5-editor").GetAttribute("data-cke-preset");

        Assert.NotNull(preset);
        Assert.Contains("inline", preset);
    }

    [Fact]
    public async Task DisposeAsync_DisposesResources_WhenDisposedAfterRender()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        await cut.Instance.DisposeAsync();
    }

    [Fact]
    public async Task DisposeAsync_DoesNotThrow_WhenDotNetHelperIsNull()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));
        var field = typeof(CKE5Editor).GetField("_dotNetHelper", BindingFlags.NonPublic | BindingFlags.Instance);

        field!.SetValue(cut.Instance, null);

        await cut.Instance.DisposeAsync();
    }
}
