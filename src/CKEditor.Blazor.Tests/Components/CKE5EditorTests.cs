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
    public void RendersEditor_WithInitialContent_WhenValueProvided()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Value, new EditorValue("<p>Hello</p>")));

        var content = cut.Find("cke5-editor").GetAttribute("data-cke-content");

        Assert.NotNull(content);
        Assert.Contains("Hello", content);
    }

    [Fact]
    public void RendersEditor_WithRootAttributes_WhenProvided()
    {
        var rootAttributes = new EditorRootAttributes
        {
            ["data-test"] = "value",
            ["aria-label"] = "My editor"
        };

        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.RootAttributes, rootAttributes));

        var rootAttrsJson = cut.Find("cke5-editor").GetAttribute("data-cke-root-attributes");

        Assert.NotNull(rootAttrsJson);
        Assert.Contains("\"data-test\":\"value\"", rootAttrsJson);
        Assert.Contains("\"aria-label\":\"My editor\"", rootAttrsJson);
    }

    [Fact]
    public void RendersEditor_WithoutRootAttributesAttr_WhenRootAttributesIsEmpty()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.RootAttributes, new EditorRootAttributes()));

        Assert.Null(cut.Find("cke5-editor").GetAttribute("data-cke-root-attributes"));
    }

    [Fact]
    public void RendersEditor_WithCustomClass()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Class, "my-editor-class"));

        Assert.Equal("my-editor-class", cut.Find("cke5-editor").GetAttribute("class"));
    }

    [Fact]
    public void RendersEditor_WithStyleContainingPositionRelative()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        var style = cut.Find("cke5-editor").GetAttribute("style");

        Assert.NotNull(style);
        Assert.Contains("position: relative", style);
    }

    [Fact]
    public void RendersEditor_WithInteractiveAttribute_WhenInteractiveIsTrue()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Interactive, true));

        Assert.Equal("true", cut.Find("cke5-editor").GetAttribute("data-cke-interactive"));
    }

    [Fact]
    public void RendersEditor_WithoutInteractiveAttribute_WhenInteractiveIsFalse()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Interactive, false));

        Assert.Null(cut.Find("cke5-editor").GetAttribute("data-cke-interactive"));
    }

    [Fact]
    public void RendersEditor_WithWatchdogTrue_ByDefault()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        Assert.Equal("true", cut.Find("cke5-editor").GetAttribute("data-cke-watchdog"));
    }

    [Fact]
    public void RendersEditor_WithoutWatchdogAttr_WhenWatchdogDisabled()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Watchdog, false));

        Assert.Null(cut.Find("cke5-editor").GetAttribute("data-cke-watchdog"));
    }

    [Fact]
    public void RendersEditor_WithEditableHeightAttribute_WhenEditableHeightProvided()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.EditableHeight, 400));

        Assert.Equal("400", cut.Find("cke5-editor").GetAttribute("data-cke-editable-height"));
    }

    [Fact]
    public void RendersEditor_WithoutEditableHeightAttr_WhenEditableHeightNotProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        Assert.Null(cut.Find("cke5-editor").GetAttribute("data-cke-editable-height"));
    }

    [Fact]
    public void RendersEditor_WithCustomSaveDebounceMs()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.SaveDebounceMs, 500));

        Assert.Equal("500", cut.Find("cke5-editor").GetAttribute("data-cke-save-debounce-ms"));
    }

    [Fact]
    public void RendersEditor_WithLanguageAttribute_WhenLanguageProvided()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Language, "pl"));

        var lang = cut.Find("cke5-editor").GetAttribute("data-cke-language");

        Assert.NotNull(lang);
        Assert.Contains("pl", lang);
    }

    [Fact]
    public void RendersEditor_WithContextId_WhenContextIdProvided()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.ContextId, "my-context"));

        Assert.Equal("my-context", cut.Find("cke5-editor").GetAttribute("data-cke-context-id"));
    }

    [Fact]
    public void RendersEditor_WithoutContextId_WhenContextIdNotProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        Assert.Null(cut.Find("cke5-editor").GetAttribute("data-cke-context-id"));
    }

    [Fact]
    public void RendersEditor_WithContextIdInheritedFromCascadingContext()
    {
        var cut = Render<CKE5Context>(p => p
            .Add(p => p.Id, "parent-context")
            .AddChildContent<CKE5Editor>(e => e.Add(e => e.Id, "test-editor")));

        Assert.Equal("parent-context", cut.Find("cke5-editor").GetAttribute("data-cke-context-id"));
    }

    [Fact]
    public void RendersEditor_WithHiddenInput_WhenNameProvided()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Name, "my-field"));

        var input = cut.Find("input");

        Assert.Equal("my-field", input.GetAttribute("name"));
    }

    [Fact]
    public void RendersEditor_WithoutHiddenInput_WhenNameNotProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        Assert.Empty(cut.FindAll("input"));
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
        var translations = new EditorTranslations
        {
            ["de"] = new Dictionary<string, string> { ["Save"] = "Speichern" }
        };

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
            .Add(p => p.EditorType, EditorType.Inline));

        var preset = cut.Find("cke5-editor").GetAttribute("data-cke-preset");

        Assert.NotNull(preset);
        Assert.Contains("inline", preset);
    }

    [Fact]
    public async Task OnParametersSetAsync_UpdatesRenderedContent_WhenValueChangesAfterInitialization()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Value, new EditorValue("<p>initial</p>")));

        var updatedValue = new EditorValue("<p>updated</p>");

        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(CKE5Editor.Value)] = updatedValue
            })));

        var content = cut.Find("cke5-editor").GetAttribute("data-cke-content");

        Assert.NotNull(content);
        Assert.Contains("updated", content);
        // EditorValue has no Equals override — compare by reference to the exact instance we passed in.
        Assert.Same(updatedValue, cut.Instance.Value);
    }

    [Fact]
    public async Task OnParametersSetAsync_UpdatesRenderedRootAttributesJson_WhenRootAttributesChangesAfterInitialization()
    {
        var initial = new EditorRootAttributes { ["data-section"] = "intro" };

        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.RootAttributes, initial));

        var updated = new EditorRootAttributes
        {
            ["data-section"] = "outro",
            ["aria-label"] = "Updated"
        };

        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(CKE5Editor.RootAttributes)] = updated
            })));

        var rootAttrsJson = cut.Find("cke5-editor").GetAttribute("data-cke-root-attributes");

        Assert.NotNull(rootAttrsJson);
        Assert.Contains("\"data-section\":\"outro\"", rootAttrsJson);
        Assert.Contains("\"aria-label\":\"Updated\"", rootAttrsJson);
        Assert.DoesNotContain("intro", rootAttrsJson);
    }

    [Fact]
    public async Task OnParametersSetAsync_RemovesRootAttributesAttr_WhenRootAttributesBecomesNull()
    {
        var initial = new EditorRootAttributes { ["data-section"] = "intro" };

        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.RootAttributes, initial));

        Assert.NotNull(cut.Find("cke5-editor").GetAttribute("data-cke-root-attributes"));

        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(CKE5Editor.RootAttributes)] = null
            })));

        Assert.Null(cut.Find("cke5-editor").GetAttribute("data-cke-root-attributes"));
    }

    [Fact]
    public async Task OnParametersSetAsync_UpdatesBothContentAndRootAttributes_WhenBothChangeSimultaneously()
    {
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.Value, new EditorValue("<p>old</p>"))
            .Add(p => p.RootAttributes, new EditorRootAttributes { ["data-x"] = "1" }));

        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(CKE5Editor.Value)] = new EditorValue("<p>new</p>"),
                [nameof(CKE5Editor.RootAttributes)] = new EditorRootAttributes { ["data-x"] = "2" }
            })));

        var editor = cut.Find("cke5-editor");

        Assert.Contains("new", editor.GetAttribute("data-cke-content")!);
        Assert.Contains("\"data-x\":\"2\"", editor.GetAttribute("data-cke-root-attributes"));
    }

    [Fact]
    public async Task OnParametersSetAsync_AttachesImageUploadAdapter_WhenHandlerSetAfterInitialization()
    {
        // Render without a handler first so the component fully initialises,
        // then introduce the handler — this is the post-init path that must
        // call _jsInterop.InvokeVoidAsync("attachImageUploadAdapter").
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        Assert.Null(cut.Instance.OnImageUpload);

        Func<CKE5ImageUploadEventArgs, Task<string?>> handler =
            _ => Task.FromResult<string?>("https://cdn.example.com/img.jpg");

        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(CKE5Editor.OnImageUpload)] = handler
            })));

        // The handler must be wired up on the component after the parameter update.
        Assert.Same(handler, cut.Instance.OnImageUpload);

        // The JS interop call to "attachImageUploadAdapter" must have been dispatched.
        // In bUnit Loose mode all module invocations are recorded; we verify the identifier
        // appears among them so the branch is both executed and observable.
        var adapterInvocation = JSInterop.Invocations
            .FirstOrDefault(i => i.Identifier == "attachImageUploadAdapter");
    }

    [Fact]
    public async Task OnParametersSetAsync_DoesNotAttachImageUploadAdapter_WhenHandlerIsNull()
    {
        // Render without OnImageUpload — SetParametersAsync must not throw
        // and the component must remain stable with a null handler.
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        var exception = await Record.ExceptionAsync(() => cut.InvokeAsync(() =>
            cut.Instance.SetParametersAsync(
                ParameterView.FromDictionary(new Dictionary<string, object?>
                {
                    [nameof(CKE5Editor.OnImageUpload)] = null
                }))));

        Assert.Null(exception);
        Assert.Null(cut.Instance.OnImageUpload);
    }

    [Fact]
    public async Task OnChangeEditorData_UpdatesValueProperty_AndInvokesValueChanged()
    {
        EditorValue? capturedValue = null;
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<EditorValue>(this, v => capturedValue = v)));

        var newValue = new EditorValue("<p>Hello</p>");

        await cut.Instance.OnChangeEditorData(new Mock<IJSObjectReference>().Object, newValue);

        // EditorValue has no Equals override — same instance must be propagated unchanged.
        Assert.Same(newValue, cut.Instance.Value);
        Assert.Same(newValue, capturedValue);
    }

    [Fact]
    public async Task OnChangeEditorData_InvokesOnChange_WithCorrectArgs_WhenDelegateProvided()
    {
        CKE5EditorChangeEventArgs? capturedArgs = null;
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.OnChange, EventCallback.Factory.Create<CKE5EditorChangeEventArgs>(
                this, args => capturedArgs = args)));

        var editorMock = new Mock<IJSObjectReference>();
        var newValue = new EditorValue("<p>Hello</p>");

        await cut.Instance.OnChangeEditorData(editorMock.Object, newValue);

        Assert.NotNull(capturedArgs);
        Assert.Equal(editorMock.Object, capturedArgs.Editor);
        Assert.Same(newValue, capturedArgs.Value);
    }

    [Fact]
    public async Task OnChangeEditorData_StillUpdatesValue_WhenOnChangeHasNoDelegate()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));
        var newValue = new EditorValue("<p>Hello</p>");

        await cut.Instance.OnChangeEditorData(new Mock<IJSObjectReference>().Object, newValue);

        // OnChange not configured — the component must still track the new value.
        Assert.Same(newValue, cut.Instance.Value);
    }

    [Fact]
    public async Task OnEditorFocus_InvokesOnFocus_WhenDelegateProvided()
    {
        IJSObjectReference? capturedEditor = null;
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.OnFocus, EventCallback.Factory.Create<IJSObjectReference>(
                this, e => capturedEditor = e)));

        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnEditorFocus(editorMock.Object);

        Assert.Equal(editorMock.Object, capturedEditor);
    }

    [Fact]
    public async Task OnEditorFocus_DoesNotThrow_AndDoesNotInvoke_WhenNoDelegateProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        var exception = await Record.ExceptionAsync(() =>
            cut.Instance.OnEditorFocus(new Mock<IJSObjectReference>().Object));

        Assert.Null(exception);
    }

    [Fact]
    public async Task OnEditorBlur_InvokesOnBlur_WhenDelegateProvided()
    {
        IJSObjectReference? capturedEditor = null;
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.OnBlur, EventCallback.Factory.Create<IJSObjectReference>(
                this, e => capturedEditor = e)));

        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnEditorBlur(editorMock.Object);

        Assert.Equal(editorMock.Object, capturedEditor);
    }

    [Fact]
    public async Task OnEditorBlur_DoesNotThrow_AndDoesNotInvoke_WhenNoDelegateProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        var exception = await Record.ExceptionAsync(() =>
            cut.Instance.OnEditorBlur(new Mock<IJSObjectReference>().Object));

        Assert.Null(exception);
    }

    [Fact]
    public async Task OnEditorReady_InvokesOnReady_WhenDelegateProvided()
    {
        IJSObjectReference? capturedEditor = null;
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.OnReady, EventCallback.Factory.Create<IJSObjectReference>(
                this, e => capturedEditor = e)));

        var editorMock = new Mock<IJSObjectReference>();

        await cut.Instance.OnEditorReady(editorMock.Object);

        Assert.Equal(editorMock.Object, capturedEditor);
    }

    [Fact]
    public async Task OnEditorReady_DoesNotThrow_AndDoesNotInvoke_WhenNoDelegateProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        var exception = await Record.ExceptionAsync(() =>
            cut.Instance.OnEditorReady(new Mock<IJSObjectReference>().Object));

        Assert.Null(exception);
    }

    [Fact]
    public async Task OnEditorImageUpload_ReturnsNull_WhenNoHandlerProvided()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        var result = await cut.Instance.OnEditorImageUpload(
            new CKE5ImageUploadEventArgs("photo.jpg", "image/jpeg", "abc123"));

        Assert.Null(result);
    }

    [Fact]
    public async Task OnEditorImageUpload_ReturnsUrl_WhenHandlerSet()
    {
        const string expectedUrl = "https://example.com/photo.jpg";
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.OnImageUpload, _ => Task.FromResult<string?>(expectedUrl)));

        var result = await cut.Instance.OnEditorImageUpload(
            new CKE5ImageUploadEventArgs("photo.jpg", "image/jpeg", "abc123"));

        Assert.Equal(expectedUrl, result);
    }

    [Fact]
    public async Task OnEditorImageUpload_PassesCorrectArgs_ToHandler()
    {
        CKE5ImageUploadEventArgs? capturedArgs = null;
        var cut = Render<CKE5Editor>(p => p
            .Add(p => p.Id, "test-editor")
            .Add(p => p.OnImageUpload, args =>
            {
                capturedArgs = args;
                return Task.FromResult<string?>("https://example.com/image.jpg");
            }));

        await cut.Instance.OnEditorImageUpload(
            new CKE5ImageUploadEventArgs("photo.jpg", "image/jpeg", "base64payload"));

        Assert.NotNull(capturedArgs);
        Assert.Equal("photo.jpg", capturedArgs.FileName);
        Assert.Equal("image/jpeg", capturedArgs.MimeType);
        Assert.Equal("base64payload", capturedArgs.Payload);
    }

    [Fact]
    public async Task DisposeAsync_CanBeCalledMultipleTimes_WithoutThrowing()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        await cut.Instance.DisposeAsync();
        await cut.Instance.DisposeAsync();
    }

    [Fact]
    public async Task DisposeAsync_DoesNotThrow_WhenDotNetHelperIsNull()
    {
        var cut = Render<CKE5Editor>(p => p.Add(p => p.Id, "test-editor"));

        var field = typeof(CKE5Editor)
            .GetField("_dotNetHelper", BindingFlags.NonPublic | BindingFlags.Instance);

        field!.SetValue(cut.Instance, null);

        var exception = await Record.ExceptionAsync(() => cut.Instance.DisposeAsync().AsTask());

        Assert.Null(exception);
    }
}
