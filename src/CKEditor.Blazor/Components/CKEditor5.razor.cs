using System.Text.Json;
using System.Text.Json.Serialization;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Preset;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 Main Component.
/// Renders a CKEditor instance with configurable options.
/// </summary>
public partial class CKEditor5 : ComponentBase, IAsyncDisposable
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private IJSObjectReference? _jsModule;

    private IJSObjectReference? _jsInterop;

    private DotNetObjectReference<CKEditor5>? _dotNetHelper;

    private bool _isInitializing = true;

    /// <summary>
    /// The initial value of the editor. Can be a string or a dictionary for multiroot editors.
    /// </summary>
    [Parameter]
    public CKEditorValue? Value { get; set; }

    /// <summary>
    /// Event callback for two-way binding of `Value`.
    /// </summary>
    [Parameter]
    public EventCallback<CKEditorValue?> ValueChanged { get; set; }

    /// <summary>
    /// The preset name or object to use (default: 'default').
    /// </summary>
    [Parameter]
    public object? Preset { get; set; } = "default";

    /// <summary>
    /// Whether to enable the watchdog feature (default: true).
    /// </summary>
    [Parameter]
    public bool Watchdog { get; set; } = true;

    /// <summary>
    /// Optional name for the input field.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Whether the input is required.
    /// </summary>
    [Parameter]
    public bool? Required { get; set; }

    /// <summary>
    /// Optional height for the editable area in pixels.
    /// </summary>
    [Parameter]
    public int? EditableHeight { get; set; }

    /// <summary>
    /// The debounce time in milliseconds for saving changes.
    /// </summary>
    [Parameter]
    public int SaveDebounceMs { get; set; } = 250;

    /// <summary>
    /// Optional CSS class for the editor container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Optional inline styles for the editor container.
    /// </summary>
    [Parameter]
    public string? Style { get; set; } = "display: block; width: 100%;";

    /// <summary>
    /// Optional ID for the editor instance.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Optional context ID for multiple editors sharing a context.
    /// </summary>
    [Parameter]
    public string? ContextId { get; set; }

    /// <summary>
    /// Optional language configuration (string or object).
    /// </summary>
    [Parameter]
    public object? Language { get; set; }

    /// <summary>
    /// Optional editor configuration overrides (shallow replace).
    /// </summary>
    [Parameter]
    public Dictionary<string, object>? Config { get; set; }

    /// <summary>
    /// Optional editor configuration to merge (deep merge).
    /// </summary>
    [Parameter]
    public Dictionary<string, object>? MergeConfig { get; set; }

    /// <summary>
    /// Optional custom translations dictionary.
    /// </summary>
    [Parameter]
    public Dictionary<string, string>? CustomTranslations { get; set; }

    /// <summary>
    /// Optional editor type to use (e.g., "classic", "inline", "balloon", "decoupled", "multiroot").
    /// </summary>
    [Parameter]
    public string? EditorType { get; set; }

    /// <summary>
    /// Optional child content to render inside the editor component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether the editor should be interactive and bootstrap automatically. Default is false.
    /// </summary>
    [Parameter]
    public bool Interactive { get; set; } = false;

    [Inject]
    private ConfigManager ConfigManager { get; set; } = default!;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    private string? StyleValue { get; set; }

    private string? PresetJson { get; set; }

    private string? ValueJson { get; set; }

    private string? LanguageJson { get; set; }

    private bool ShowInput { get; set; }

    private Dictionary<string, object> AdditionalAttributes { get; set; } = [];

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        _dotNetHelper?.Dispose();

        if (_jsInterop is not null)
        {
            await _jsInterop.InvokeVoidAsync("unmount");
            await _jsInterop.DisposeAsync();
        }

        if (_jsModule is not null)
        {
            await _jsModule.DisposeAsync();
        }
    }

    /// <summary>
    /// Method invoked from JS interop to update the Value based on editor data changes.
    /// </summary>
    /// <param name="roots">The new editor data value (from JS interop).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [JSInvokable]
    public async Task OnChangeEditorData(CKEditorValue roots)
    {
        Value = roots;
        await ValueChanged.InvokeAsync(Value);
    }

    protected override void OnInitialized()
    {
        Id ??= $"cke5-{Guid.NewGuid():N}";
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_isInitializing && _jsInterop is not null)
        {
            await _jsInterop.InvokeVoidAsync("setValue", Value);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetHelper = DotNetObjectReference.Create(this);
            _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "ckeditor5-blazor");
            _jsInterop = await _jsModule.InvokeAsync<IJSObjectReference>("createEditorBlazorInterop", Id, _dotNetHelper);
            _isInitializing = false;
        }
    }

    protected override void OnParametersSet()
    {
        var preset = ResolvePreset();

        StyleValue = $"position: relative; {Style}";
        ShowInput = !preset.EditorType.IsDecoupledOrMultiroot();

        PresetJson = JsonSerializer.Serialize(preset, _jsonOptions);
        ValueJson = JsonSerializer.Serialize(Value, _jsonOptions);
        LanguageJson = JsonSerializer.Serialize(Blazor.Preset.Language.Parse(Language), _jsonOptions);

        AdditionalAttributes = GetAttributes();
    }

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

    private Dictionary<string, object> GetAttributes()
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
