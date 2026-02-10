namespace CKEditor.Blazor.Preset;

/// <summary>
/// Represents the type of CKEditor instance.
/// </summary>
public enum EditorType
{
    /// <summary>
    /// Classic editor with toolbar above the editing area.
    /// </summary>
    Classic,

    /// <summary>
    /// Inline editor that activates on click.
    /// </summary>
    Inline,

    /// <summary>
    /// Balloon editor with a floating toolbar.
    /// </summary>
    Balloon,

    /// <summary>
    /// Decoupled editor with separate toolbar and editable areas.
    /// </summary>
    Decoupled,

    /// <summary>
    /// Multiroot editor with multiple editable areas.
    /// </summary>
    Multiroot
}
