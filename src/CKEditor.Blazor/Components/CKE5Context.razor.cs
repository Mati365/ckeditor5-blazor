using System.Text.Json;
using System.Text.Json.Serialization;
using CKEditor.Blazor.Serialization;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 Context Component.
/// Renders a CKEditor context that can be shared among multiple editors.
/// </summary>
public partial class CKE5Context : ComponentBase, IAsyncDisposable, ICKE5InteractiveComponent
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly CKE5ComponentJsInterop _jsInterop = new();

    /// <summary>
    /// Reference to the root DOM element of this component, captured via <c>@ref</c>.
    /// Passed to JS interop so it can mount the context on the correct node.
    /// </summary>
    private ElementReference _elementRef;

    /// <summary>
    /// The language code for the context (default: <c>'en'</c>).
    /// </summary>
    [Parameter]
    public string? Language { get; set; } = "en";

    /// <summary>
    /// The context preset name or configuration object to use (default: <c>'default'</c>).
    /// </summary>
    [Parameter]
    public object? ContextPreset { get; set; }

    /// <summary>
    /// Optional child content to render inside the context component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional HTML ID for the context instance.
    /// When not provided, a unique ID is generated automatically in <see cref="OnInitialized"/>.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <inheritdoc cref="ICKE5InteractiveComponent.Interactive"/>
    [Parameter]
    public bool Interactive { get; set; } = false;

    [Inject]
    private ConfigManager ConfigManager { get; set; } = default!;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    private string LanguageJson => JsonSerializer.Serialize(LanguageParser.Parse(Language), _jsonOptions);

    private string ContextJson => JsonSerializer.Serialize(
        ConfigManager.ResolveContext(ContextPreset ?? "default"), _jsonOptions);

    /// <summary>
    /// Disposes the JS interop instance.
    /// </summary>
    /// <returns>A task representing the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _jsInterop.DisposeAsync();
    }

    /// <summary>
    /// Generates a unique <see cref="Id"/> when none is provided by the consumer.
    /// </summary>
    protected override void OnInitialized() => Id ??= $"cke5-context-{Guid.NewGuid():N}";

    /// <summary>
    /// On the first render, initializes the JS interop by invoking
    /// <c>createContextBlazorInterop</c> with the root element reference.
    /// No <see cref="DotNetObjectReference{T}"/> is passed because this component
    /// does not expose any JS-invokable callbacks.
    /// </summary>
    /// <param name="firstRender">
    /// <see langword="true"/> only on the initial render of the component.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _jsInterop.InitializeAsync(JS, "createContextBlazorInterop", _elementRef);
        }
    }
}
