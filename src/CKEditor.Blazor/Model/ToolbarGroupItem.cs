namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents a grouped toolbar item with a label and nested toolbar item names.
/// Create instances via <see cref="Toolbar.Group"/>.
/// </summary>
/// <param name="Label">The visible label of the group (shown as a dropdown button).</param>
/// <param name="Items">The toolbar item names inside this group.</param>
public sealed record ToolbarGroupItem(string Label, IReadOnlyList<string> Items);
