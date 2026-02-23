using System.Text.Json;
using System.Text.Json.Serialization;
using CKEditor.Blazor.Extensions;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Serialization;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 Main Component.
/// Renders a CKEditor instance with configurable options.
/// </summary>
public partial class Editor : ComponentBase, IAsyncDisposable
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private readonly CKComponentJsInterop _jsInterop = new();

    private DotNetObjectReference<Editor>? _dotNetHelper;

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
    public EventCallback<EditorValue?> ValueChanged { get; set; }

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
    /// Optional dictionary of custom UI translations keyed by the original string.
    /// </summary>
    [Parameter]
    public Dictionary<string, string>? CustomTranslations { get; set; }

    /// <summary>
    /// Optional editor type override (e.g. <c>"classic"</c>, <c>"inline"</c>,
    /// <c>"balloon"</c>, <c>"decoupled"</c>, <c>"multiroot"</c>).
    /// </summary>
    [Parameter]
    public string? EditorType { get; set; }

    /// <summary>
    /// Optional child content rendered inside the editor's custom element,
    /// useful for injecting toolbar slots in decoupled or multiroot layouts.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// When <see langword="true"/>, the editor bootstraps itself automatically via
    /// the JS Web Component without waiting for the Blazor interop initialization.
    /// Default is <see langword="false"/>.
    /// </summary>
    [Parameter]
    public bool Interactive { get; set; } = false;

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

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private ConfigManager ConfigManager { get; set; } = default!;

    private string? StyleValue { get; set; }

    private string? PresetJson { get; set; }

    private string? ValueJson { get; set; }

    private string? LanguageJson { get; set; }

    private bool ShowInput { get; set; }

    private Dictionary<string, object> AdditionalAttributes { get; set; } = [];

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
    /// <param name="roots">The new editor data emitted by the JS interop layer.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [JSInvokable]
    public async Task OnChangeEditorData(EditorValue roots)
    {
        Value = roots;
        await ValueChanged.InvokeAsync(Value);
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
    /// Generates a unique <see cref="Id"/> when none is provided by the consumer.
    /// </summary>
    protected override void OnInitialized()
    {
        Id ??= $"cke5-{Guid.NewGuid():N}";
    }

    /// <summary>
    /// Recomputes all serialized JSON attributes and the additional HTML attributes
    /// dictionary whenever bound parameters change.
    /// </summary>
    protected override void OnParametersSet()
    {
        var preset = ResolvePreset();

        StyleValue = $"position: relative; {Style}";
        ShowInput = !EditorTypeExtensions.IsDecoupledOrMultiroot(preset.EditorType);

        PresetJson = JsonSerializer.Serialize(preset, _jsonOptions);
        ValueJson = JsonSerializer.Serialize(Value, _jsonOptions);
        LanguageJson = JsonSerializer.Serialize(LanguageParser.Parse(Language), _jsonOptions);

        AdditionalAttributes = BuildAdditionalAttributes();
    }

    /// <summary>
    /// Forwards the current <see cref="Value"/> to the JS interop whenever parameters change,
    /// but only after the component has been fully initialized.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnParametersSetAsync()
    {
        if (!_jsInterop.IsInitializing && Value is not null)
        {
            await _jsInterop.InvokeVoidAsync("setValue", Value);
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
            preset = preset.OfConfig(Config);
        }

        if (MergeConfig != null)
        {
            preset = preset.OfMergedConfig(MergeConfig);
        }

        if (CustomTranslations != null)
        {
            preset = preset.OfCustomTranslations(CustomTranslations);
        }

        if (!string.IsNullOrWhiteSpace(EditorType))
        {
            var editorType = Enum.Parse<EditorType>(EditorType, ignoreCase: true);
            preset = preset.OfEditorType(editorType);
        }

        return preset;
    }

    /// <summary>
    /// Builds the dictionary of <c>data-cke-*</c> HTML attributes that are conditionally
    /// rendered on the editor's custom element based on the current parameter values.
    /// </summary>
    /// <returns>
    /// A dictionary of attribute name/value pairs to be spread via <c>@attributes</c>.
    /// </returns>
    private Dictionary<string, object> BuildAdditionalAttributes()
    {
        var attributes = new Dictionary<string, object>();

        if (Interactive)
        {
            attributes["data-cke-interactive"] = "true";
        }

        if (Watchdog)
        {
            attributes["data-cke-watchdog"] = "true";
        }

        if (!string.IsNullOrEmpty(ContextId))
        {
            attributes["data-cke-context-id"] = ContextId;
        }

        if (EditableHeight.HasValue)
        {
            attributes["data-cke-editable-height"] = EditableHeight.Value;
        }

        return attributes;
    }
}
