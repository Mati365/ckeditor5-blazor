using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 UI Part Component.
/// Renders a UI part container (e.g., toolbar, menubar) for decoupled editors.
/// The component mounts itself into the JS interop so that CKEditor can attach
/// the requested UI part to this DOM node.
/// </summary>
public partial class UIPart : ComponentBase, IAsyncDisposable
{
    private readonly CKComponentJsInterop _jsInterop = new();

    private DotNetObjectReference<UIPart>? _dotNetHelper;

    /// <summary>
    /// Reference to the root DOM element of this component, captured via <c>@ref</c>.
    /// Passed to JS interop so it can mount the UI part on the correct node.
    /// </summary>
    private ElementReference _elementRef;

    /// <summary>
    /// The name of the UI part (e.g., <c>"toolbar"</c>, <c>"menubar"</c>).
    /// </summary>
    [Parameter]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The HTML ID of the parent <see cref="Editor"/> instance this UI part belongs to.
    /// </summary>
    [Parameter]
    public string? EditorId { get; set; }

    /// <summary>
    /// Optional CSS class applied to the UI part container element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Optional inline styles applied to the UI part container element.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Optional HTML ID for the UI part instance.
    /// When not provided, a unique ID is generated automatically in <see cref="OnInitialized"/>.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// When <see langword="true"/>, the UI part bootstraps itself automatically via
    /// the JS Web Component without waiting for the Blazor interop initialization.
    /// Default is <see langword="false"/>.
    /// </summary>
    [Parameter]
    public bool Interactive { get; set; } = false;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

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
    /// Generates a unique <see cref="Id"/> when none is provided by the consumer.
    /// </summary>
    protected override void OnInitialized() => Id ??= $"cke5-ui-part-{Guid.NewGuid():N}";

    /// <summary>
    /// On the first render, creates a <see cref="DotNetObjectReference{T}"/> and initializes
    /// the JS interop by invoking <c>createUiPartBlazorInterop</c> with the root element reference.
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
            await _jsInterop.InitializeAsync(JS, "createUIPartBlazorInterop", _elementRef, _dotNetHelper);
        }
    }
}
