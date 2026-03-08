namespace CKEditor.Blazor.Model;

/// <summary>
/// Provides helpers for building CKEditor toolbar configurations using a mini DSL.
/// </summary>
/// <example>
/// <code>
/// new PresetConfig()
///     .WithPlugins("Essentials", "Bold", "Italic", "Undo")
///     .WithToolbar(
///         "bold", "italic",
///         Toolbar.Separator,
///         Toolbar.Group("Text Style", "strikethrough", "superscript"),
///         Toolbar.Separator,
///         "undo", "redo"
///     );
/// </code>
/// </example>
public static class Toolbar
{
    /// <summary>
    /// The standard toolbar separator item (<c>"|"</c>).
    /// </summary>
    public const string Separator = "|";

    /// <summary>
    /// Creates a toolbar group with a visible label and nested items.
    /// </summary>
    /// <param name="label">The visible label of the group (shown as a dropdown button).</param>
    /// <param name="items">The toolbar item names inside this group.</param>
    /// <returns>A <see cref="ToolbarGroupItem"/> representing the group.</returns>
    public static ToolbarGroupItem Group(string label, params string[] items) => new(label, items);
}
