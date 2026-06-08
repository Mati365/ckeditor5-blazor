using System.Text.Json;
using System.Text.Json.Serialization;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Events;
using CKEditor.Blazor.Serialization;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 Main Component.
/// Renders a CKEditor instance with configurable options.
/// </summary>
public partial class CKE5Editor : ComponentBase, IAsyncDisposable, ICKE5InteractiveComponent
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private readonly CKE5ComponentJsInterop _jsInterop = new();

    private DotNetObjectReference<CKE5Editor>? _dotNetHelper;

    /// <summary>
    /// Reference to the root DOM element of this component, captured via <c>@ref</c>.
    /// Passed to JS interop so it can locate and mount the editor on the correct node.
    /// </summary>
    private ElementReference _elementRef;

    /// <summary>
    /// The initial value of the editor. Can be a string or a dictionary for multiroot editors.
    /// </summary>
    [Parameter]
    public EditorValue? Value { get; set; }

    /// <summary>
    /// Event callback for two-way binding of <see cref="Value"/>.
    /// </summary>
    [Parameter]
    public EventCallback<EditorValue> ValueChanged { get; set; }

    /// <summary>
    /// Optional event callback invoked whenever the editor data changes.
    /// This callback is invoked in addition to <see cref="ValueChanged"/>,
    /// allowing consumers to react to change events without participating in
    /// two‑way binding.
    ///
    /// The callback now receives a <see cref="CKE5EditorChangeEventArgs"/>
    /// instance containing both the new value and a JS object reference to the
    /// underlying editor.
    /// </summary>
    [Parameter]
    public EventCallback<CKE5EditorChangeEventArgs> OnChange { get; set; }

    /// <summary>
    /// The preset name or object to use (default: <c>'default'</c>).
    /// </summary>
    [Parameter]
    public object? Preset { get; set; } = "default";

    /// <summary>
    /// Whether to enable the watchdog feature (default: <see langword="true"/>).
    /// </summary>
    [Parameter]
    public bool Watchdog { get; set; } = true;

    /// <summary>
    /// Optional name for the hidden input field used in form submissions.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Whether the hidden input field is required for form validation.
    /// </summary>
    [Parameter]
    public bool? Required { get; set; }

    /// <summary>
    /// Optional fixed height for the editable area in pixels.
    /// </summary>
    [Parameter]
    public int? EditableHeight { get; set; }

    /// <summary>
    /// The debounce time in milliseconds before the editor propagates content changes.
    /// </summary>
    [Parameter]
    public int SaveDebounceMs { get; set; } = 250;

    /// <summary>
    /// Optional CSS class applied to the outermost editor container element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Optional inline styles applied to the outermost editor container element.
    /// </summary>
    [Parameter]
    public string? Style { get; set; } = "display: block; width: 100%;";

    /// <summary>
    /// Optional HTML ID for the editor instance.
    /// When not provided, a unique ID is generated automatically in <see cref="OnInitialized"/>.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Optional context ID used when multiple editors share a single CKEditor context.
    /// </summary>
    [Parameter]
    public string? ContextId { get; set; }

    /// <summary>
    /// The parent <see cref="CKE5Context"/> if this editor is nested inside one.
    /// </summary>
    [CascadingParameter]
    public CKE5Context? Context { get; set; }

    /// <summary>
    /// Optional language configuration. Accepts a language code string or a language config object.
    /// </summary>
    [Parameter]
    public object? Language { get; set; }

    /// <summary>
    /// Optional editor configuration that performs a shallow replace of the resolved preset config.
    /// </summary>
    [Parameter]
    public Dictionary<string, object>? Config { get; set; }

    /// <summary>
    /// Optional editor configuration that is deep-merged into the resolved preset config.
    /// </summary>
    [Parameter]
    public Dictionary<string, object>? MergeConfig { get; set; }

    /// <summary>
    /// Optional dictionary of custom UI translations.
    /// The outer key is a language code (e.g., <c>"en"</c>, <c>"pl"</c>),
    /// and the inner dictionary maps original UI strings to their translated equivalents.
    /// </summary>
    [Parameter]
    public EditorTranslations? CustomTranslations { get; set; }

    /// <summary>
    /// Optional editor type override.
    /// </summary>
    [Parameter]
    public EditorType? EditorType { get; set; }

    /// <summary>
    /// Optional child content rendered inside the editor's custom element,
    /// useful for injecting toolbar slots in decoupled or multiroot layouts.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc cref="ICKE5InteractiveComponent.Interactive"/>
    [Parameter]
    public bool Interactive { get; set; } = false;

    /// <summary>
    /// Optional set of root attributes to associate with this editable root.
    /// Values can be strings, numbers, booleans, or any JSON-serializable object.
    /// Serialized to JSON and passed via the <c>data-cke-root-attributes</c> attribute.
    /// </summary>
    [Parameter]
    public EditorRootAttributes? RootAttributes { get; set; }

    /// <summary>
    /// Optional name of the root element. Setting it to '$inlineRoot' enables you to
    /// use the editor in paragraph-like editing mode.
    /// </summary>
    [Parameter]
    public string RootModelElement { get; set; } = "$root";

    /// <summary>
    /// Event callback invoked when the editor gains focus.
    /// The JS object reference for the editor is provided as the callback argument.
    /// </summary>
    [Parameter]
    public EventCallback<IJSObjectReference> OnFocus { get; set; }

    /// <summary>
    /// Event callback invoked when the editor loses focus.
    /// The JS object reference for the editor is provided as the callback argument.
    /// </summary>
    [Parameter]
    public EventCallback<IJSObjectReference> OnBlur { get; set; }

    /// <summary>
    /// Event callback invoked when the editor has finished initializing and the
    /// underlying CKEditor 5 instance is available.
    /// The JS object reference for the editor is provided as the callback argument.
    /// </summary>
    [Parameter]
    public EventCallback<IJSObjectReference> OnReady { get; set; }

    /// <summary>
    /// Optional asynchronous handler invoked whenever the user uploads an image through
    /// the editor's file-repository (drag-and-drop, paste, toolbar button, etc.).
    /// The handler receives a <see cref="CKE5ImageUploadEventArgs"/> with the file name,
    /// MIME type and Base64-encoded payload, and must return the public URL that
    /// CKEditor 5 should embed in the document for the uploaded image.
    /// When <see langword="null"/> the built-in upload adapter (if any) is used instead.
    /// </summary>
    [Parameter]
    public Func<CKE5ImageUploadEventArgs, Task<string?>>? OnImageUpload { get; set; }

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private ConfigManager ConfigManager { get; set; } = default!;

    private string? StyleValue { get; set; }

    private string? PresetJson { get; set; }

    private string? ValueJson =>
        JsonSerializer.Serialize(Value, _jsonOptions);

    /// <summary>
    /// If language is not defined then the config language will be used by the frontend component.
    /// </summary>
    private string? LanguageJson =>
        Language is null
            ? null
            : JsonSerializer.Serialize(LanguageParser.Parse(Language), _jsonOptions);

    private string? RootAttributesJson =>
        RootAttributes is { Count: > 0 }
            ? JsonSerializer.Serialize(RootAttributes, _jsonOptions)
            : null;

    private string? ResolvedContextId => ContextId ?? Context?.Id;

    private bool ShowInput { get; set; }

    /// <summary>
    /// Disposes the <see cref="DotNetObjectReference{T}"/> and the JS interop instance.
    /// </summary>
    /// <returns>A task representing the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        _dotNetHelper?.Dispose();
        await _jsInterop.DisposeAsync();
    }

    /// <summary>
    /// Method invoked from JS interop to update <see cref="Value"/> based on editor data changes.
    /// </summary>
    /// <param name="editor">A JS object reference to the CKEditor instance emitting the change.</param>
    /// <param name="roots">The new editor data emitted by the JS interop layer.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [JSInvokable]
    public async Task OnChangeEditorData(IJSObjectReference editor, EditorValue roots)
    {
        Value = roots;
        await ValueChanged.InvokeAsync(Value);

        if (OnChange.HasDelegate)
        {
            var args = new CKE5EditorChangeEventArgs(editor, roots);

            await OnChange.InvokeAsync(args);
        }
    }

    /// <summary>
    /// JS-invokable callback method triggered when the editor gains focus.
    /// </summary>
    /// <param name="editor">JS object reference for the editor.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [JSInvokable]
    public async Task OnEditorFocus(IJSObjectReference editor)
    {
        if (OnFocus.HasDelegate)
        {
            await OnFocus.InvokeAsync(editor);
        }
    }

    /// <summary>
    /// JS-invokable callback method triggered when the editor loses focus.
    /// </summary>
    /// <param name="editor">JS object reference for the editor.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [JSInvokable]
    public async Task OnEditorBlur(IJSObjectReference editor)
    {
        if (OnBlur.HasDelegate)
        {
            await OnBlur.InvokeAsync(editor);
        }
    }

    /// <summary>
    /// JS-invokable callback that is called when the editor instance is ready.
    /// The <paramref name="editor"/> parameter is the JS object reference that
    /// can be used to invoke editor methods directly from .NET.
    /// </summary>
    /// <param name="editor">JS object reference for the editor.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [JSInvokable]
    public async Task OnEditorReady(IJSObjectReference editor)
    {
        if (OnReady.HasDelegate)
        {
            await OnReady.InvokeAsync(editor);
        }
    }

    /// <summary>
    /// JS-invokable callback that fires when the editor's file repository requests an image upload.
    /// Delegates to <see cref="OnImageUpload"/> when a handler has been configured and returns
    /// the URL that CKEditor 5 should embed in the document, or <see langword="null"/> if no
    /// handler is registered (which causes the upload to be rejected on the JS side).
    /// </summary>
    /// <param name="args">Upload event arguments: file name, MIME type and Base64 payload.</param>
    /// <returns>The public URL for the uploaded image, or <see langword="null"/>.</returns>
    [JSInvokable]
    public async Task<string?> OnEditorImageUpload(CKE5ImageUploadEventArgs args)
    {
        if (OnImageUpload is not null)
        {
            return await OnImageUpload(args);
        }

        return null;
    }

    /// <summary>
    /// Generates a unique <see cref="Id"/> when none is provided by the consumer.
    /// </summary>
    protected override void OnInitialized() => Id ??= $"cke5-{Guid.NewGuid():N}";

    /// <summary>
    /// Recomputes all serialized JSON attributes and the additional HTML attributes
    /// dictionary whenever bound parameters change.
    /// </summary>
    protected override void OnParametersSet()
    {
        var preset = ResolvePreset();

        StyleValue = $"position: relative; {Style}";
        ShowInput = !preset.EditorType.IsDecoupledOrMultiroot();

        PresetJson = JsonSerializer.Serialize(preset, _jsonOptions);
    }

    /// <summary>
    /// Forwards the current <see cref="Value"/> to the JS interop whenever parameters change,
    /// but only after the component has been fully initialized.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnParametersSetAsync()
    {
        if (_jsInterop.IsInitializing)
        {
            return;
        }

        if (RootAttributes is not null)
        {
            await _jsInterop.InvokeVoidAsync("setRootAttributes", RootAttributes);
        }

        if (Value is not null)
        {
            await _jsInterop.InvokeVoidAsync("setValue", Value);
        }

        if (OnImageUpload is not null)
        {
            await _jsInterop.InvokeVoidAsync("attachImageUploadAdapter");
        }
    }

    /// <summary>
    /// On the first render, creates a <see cref="DotNetObjectReference{T}"/> and initializes
    /// the JS interop by invoking <c>createEditorBlazorInterop</c> with the root element reference.
    /// </summary>
    /// <param name="firstRender">
    /// <see langword="true"/> only on the initial render of the component.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetHelper = DotNetObjectReference.Create(this);
            await _jsInterop.InitializeAsync(JS, "createEditorBlazorInterop", _elementRef, _dotNetHelper);
        }
    }

    /// <summary>
    /// Resolves the active <see cref="PresetConfig"/> by starting from the configured
    /// preset and applying any <see cref="Config"/>, <see cref="MergeConfig"/>,
    /// <see cref="CustomTranslations"/>, and <see cref="EditorType"/> overrides in order.
    /// </summary>
    /// <returns>The fully resolved <see cref="PresetConfig"/> for this editor instance.</returns>
    private PresetConfig ResolvePreset()
    {
        var preset = ConfigManager.ResolvePreset(Preset);

        if (Config != null)
        {
            preset = preset with { Config = Config };
        }

        if (MergeConfig != null)
        {
            preset = preset.WithMergedConfig(MergeConfig);
        }

        if (CustomTranslations != null)
        {
            preset = preset with { CustomTranslations = CustomTranslations };
        }

        if (EditorType is not null)
        {
            preset = preset with { EditorType = EditorType.Value };
        }

        return preset;
    }
}
