using System.Text.Json;
using System.Text.Json.Serialization;
using CKEditor.Blazor.Infrastructure;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 Context Component.
/// Renders a CKEditor context that can be shared among multiple editors.
/// </summary>
public partial class Context : ComponentBase, IAsyncDisposable
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private IJSObjectReference? _jsModule;

    /// <summary>
    /// The language code for the context (default: 'en').
    /// </summary>
    [Parameter]
    public string? Language { get; set; } = "en";

    /// <summary>
    /// The context preset name or configuration object to use (default: 'default').
    /// </summary>
    [Parameter]
    public object? ContextPreset { get; set; }

    /// <summary>
    /// Optional child content to render inside the context component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional ID for the context instance.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Whether the context should be interactive and bootstrap automatically. Default is false.
    /// </summary>
    [Parameter]
    public bool Interactive { get; set; } = false;

    [Inject]
    private ConfigManager ConfigManager { get; set; } = default!;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    private string LanguageJson => JsonSerializer.Serialize(LanguageParser.Parse(Language), _jsonOptions);

    private string ContextJson => JsonSerializer.Serialize(ConfigManager.ResolveContext(ContextPreset ?? "default"), _jsonOptions);

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_jsModule is not null)
        {
            await _jsModule.DisposeAsync();
        }
    }

    protected override void OnInitialized()
    {
        Id ??= $"cke5-context-{Guid.NewGuid():N}";
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "ckeditor5-blazor");
            await _jsModule.InvokeVoidAsync("createContextBlazorInterop", Id);
        }
    }
}
