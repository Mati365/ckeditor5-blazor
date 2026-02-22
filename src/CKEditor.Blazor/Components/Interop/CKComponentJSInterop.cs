using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CKEditor.Blazor.Components;

/// <summary>
/// Manages the JS interop lifecycle for a single CKEditor Blazor component.
/// Handles loading the <c>ckeditor5-blazor</c> ES module, invoking the component-specific
/// factory function, and disposing all JS references on teardown.
/// Intended to be used via composition — each component holds one instance as a private field.
/// </summary>
/// <remarks>
/// After <see cref="InitializeAsync"/> completes, components may call
/// <see cref="InvokeVoidAsync"/> and <see cref="InvokeAsync{T}"/> to communicate
/// with the JS-side interop object.
/// </remarks>
internal sealed class CKComponentJsInterop : IAsyncDisposable
{
    /// <summary>
    /// JS module loaded from the <c>ckeditor5-blazor</c> ES module.
    /// <see langword="null"/> until <see cref="InitializeAsync"/> completes.
    /// </summary>
    private IJSObjectReference? _jsModule;

    /// <summary>
    /// JS object reference returned by the component-specific factory function.
    /// <see langword="null"/> until <see cref="InitializeAsync"/> completes.
    /// </summary>
    private IJSObjectReference? _jsInterop;

    /// <summary>
    /// Indicates whether <see cref="InitializeAsync"/> has not yet completed.
    /// </summary>
    public bool IsInitializing { get; private set; } = true;

    /// <summary>
    /// Loads the <c>ckeditor5-blazor</c> JS module and invokes the specified factory function
    /// to create the JS-side interop object for the component.
    /// </summary>
    /// <param name="js">The JS runtime to use for all interop calls.</param>
    /// <param name="factoryFunctionName">
    /// The exported JS factory function to invoke, e.g.
    /// <c>"createEditorBlazorInterop"</c>, <c>"createEditableBlazorInterop"</c>,
    /// <c>"createUiPartBlazorInterop"</c>, or <c>"createContextBlazorInterop"</c>.
    /// </param>
    /// <param name="element">
    /// The root DOM element of the Blazor component, captured via <c>@ref</c>.
    /// Passed to JS so the factory can mount on the correct node.
    /// </param>
    /// <param name="dotNetHelper">
    /// An optional <see cref="DotNetObjectReference{T}"/> for the component, enabling JS
    /// to invoke <c>[JSInvokable]</c> C# callbacks. Pass <see langword="null"/> for components
    /// that do not expose any JS-invokable methods (e.g. <see cref="Context"/>, <see cref="UIPart"/>).
    /// </param>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    public async Task InitializeAsync(
        IJSRuntime js,
        string factoryFunctionName,
        ElementReference element,
        object? dotNetHelper = null)
    {
        _jsModule = await js.InvokeAsync<IJSObjectReference>("import", "ckeditor5-blazor");
        _jsInterop = await _jsModule.InvokeAsync<IJSObjectReference>(
            factoryFunctionName, element, dotNetHelper);
        IsInitializing = false;
    }

    /// <summary>
    /// Invokes a void JS method on the interop object.
    /// </summary>
    /// <param name="identifier">The method name to invoke on the JS interop object.</param>
    /// <param name="args">Arguments to pass to the JS method.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if called before <see cref="InitializeAsync"/> completes.
    /// </exception>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeVoidAsync(string identifier, params object?[] args)
    {
        if (_jsInterop is null)
        {
            throw new InvalidOperationException(
                $"Cannot invoke '{identifier}' before the JS interop has been initialized.");
        }

        await _jsInterop.InvokeVoidAsync(identifier, args);
    }

    /// <summary>
    /// Invokes a JS method on the interop object and returns its result.
    /// </summary>
    /// <typeparam name="TValue">The expected return type of the JS method.</typeparam>
    /// <param name="identifier">The method name to invoke on the JS interop object.</param>
    /// <param name="args">Arguments to pass to the JS method.</param>
    /// <returns>The value returned by the JS method.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if called before <see cref="InitializeAsync"/> completes.
    /// </exception>
    public async Task<TValue> InvokeAsync<TValue>(string identifier, params object?[] args)
    {
        if (_jsInterop is null)
        {
            throw new InvalidOperationException(
                $"Cannot invoke '{identifier}' before the JS interop has been initialized.");
        }

        return await _jsInterop.InvokeAsync<TValue>(identifier, args);
    }

    /// <summary>
    /// Calls <c>unmount</c> on the JS interop object and disposes all JS references.
    /// Safe to call even if <see cref="InitializeAsync"/> never completed.
    /// </summary>
    /// <returns>A task representing the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
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
}
