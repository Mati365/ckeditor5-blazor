using CKEditor.Blazor.Model.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 Editable Component.
/// Renders a standalone editable region for CKEditor, intended for use
/// inside multiroot or decoupled editor layouts where each root is managed independently.
/// </summary>
public partial class Editable : ComponentBase, IAsyncDisposable
{
    private readonly CKComponentJsInterop _jsInterop = new();

    private DotNetObjectReference<Editable>? _dotNetHelper;

    /// <summary>
    /// Reference to the root DOM element of this component, captured via <c>@ref</c>.
    /// Passed to JS interop so it can locate and mount the editable on the correct node.
    /// </summary>
    private ElementReference _elementRef;

    /// <summary>
    /// The name of the root element within the multiroot/decoupled editor.
    /// Defaults to <c>"main"</c>.
    /// </summary>
    [Parameter]
    public string RootName { get; set; } = "main";

    /// <summary>
    /// The HTML ID of the parent <see cref="Editor"/> instance this editable belongs to.
    /// </summary>
    [Parameter]
    public string? EditorId { get; set; }

    /// <summary>
    /// The initial HTML content for this editable root.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Event callback for two-way binding of <see cref="Value"/> on this editable root.
    /// </summary>
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Optional event callback that is invoked whenever the editable root's
    /// data changes. This is raised in addition to the <see cref="ValueChanged"/>
    /// callback and may be used by consumers who don't wish to participate in
    /// two‑way binding.
    ///
    /// The callback now receives a <see cref="EditableChangeEventArgs"/>
    /// instance containing both the new data and a JS object reference for the
    /// underlying editor.
    /// </summary>
    [Parameter]
    public EventCallback<EditableChangeEventArgs> OnChange { get; set; }

    /// <summary>
    /// The debounce time in milliseconds before content changes are propagated.
    /// </summary>
    [Parameter]
    public int SaveDebounceMs { get; set; } = 500;

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
    /// Optional CSS class applied to the outermost editable container element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Optional inline styles applied to the outermost editable container element.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Optional HTML ID for the editable instance.
    /// When not provided, a unique ID is generated automatically in <see cref="OnInitialized"/>.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Optional CSS class applied to the inner content <c>&lt;div&gt;</c>
    /// that serves as the actual CKEditor editable region.
    /// </summary>
    [Parameter]
    public string? InnerClass { get; set; }

    /// <summary>
    /// Optional inline styles applied to the inner content <c>&lt;div&gt;</c>
    /// that serves as the actual CKEditor editable region.
    /// </summary>
    [Parameter]
    public string? InnerStyle { get; set; }

    /// <summary>
    /// When <see langword="true"/>, the editable bootstraps itself automatically via
    /// the JS Web Component without waiting for the Blazor interop initialization.
    /// Default is <see langword="false"/>.
    /// </summary>
    [Parameter]
    public bool Interactive { get; set; } = false;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    private string StyleValue => $"position: relative;{(string.IsNullOrEmpty(Style) ? string.Empty : $" {Style}")}";

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
    /// JS-invokable callback method for handling content changes from the JS side.
    /// </summary>
    /// <param name="editor">A JS object reference to the CKEditor instance owning this editable.</param>
    /// <param name="data">The new content data from the JS side.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [JSInvokable]
    public async Task OnChangeEditableData(IJSObjectReference editor, string data)
    {
        Value = data;
        await ValueChanged.InvokeAsync(data);

        if (OnChange.HasDelegate)
        {
            var args = new EditableChangeEventArgs(RootName, editor, data);

            await OnChange.InvokeAsync(args);
        }
    }

    /// <summary>
    /// Generates a unique <see cref="Id"/> when none is provided by the consumer.
    /// </summary>
    protected override void OnInitialized()
    {
        Id ??= $"cke5-editable-{Guid.NewGuid():N}";
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
    /// the JS interop by invoking <c>createEditableBlazorInterop</c> with the root element reference.
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
            await _jsInterop.InitializeAsync(JS, "createEditableBlazorInterop", _elementRef, _dotNetHelper);
        }
    }
}
