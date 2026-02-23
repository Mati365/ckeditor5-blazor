using Microsoft.JSInterop;

namespace CKEditor.Blazor.Model.Events;

/// <summary>
/// Event arguments for the <see cref="Components.Editor"/> component when editor data changes.
/// </summary>
/// <param name="Editor">The CKEditor JS instance that emitted the change.</param>
/// <param name="Value">The new editor value (single-root string or multi-root dictionary).</param>
public sealed record EditorChangeEventArgs(IJSObjectReference Editor, EditorValue Value);
