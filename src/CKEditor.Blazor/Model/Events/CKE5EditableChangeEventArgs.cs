using Microsoft.JSInterop;

namespace CKEditor.Blazor.Model.Events;

/// <summary>
/// Event arguments for the <see cref="Components.CKE5Editable"/> component when editable data changes.
/// </summary>
/// <param name="RootName">The name of the root element that emitted the change.</param>
/// <param name="Editor">The CKEditor JS instance that owns the editable.</param>
/// <param name="Value">The updated data for the editable root.</param>
public sealed record CKE5EditableChangeEventArgs(string RootName, IJSObjectReference Editor, string Value);
