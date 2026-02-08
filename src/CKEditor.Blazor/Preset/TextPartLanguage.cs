namespace CKEditor.Blazor.Preset;

/// <summary>
/// Represents a text part language configuration.
/// </summary>
public class TextPartLanguage
{
    /// <summary>
    /// The language code.
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// The title for this language option.
    /// </summary>
    public string? Title { get; set; }
}
