namespace CKEditor.Blazor.Preset;

/// <summary>
/// Represents language configuration for CKEditor.
/// </summary>
public class Language
{
    /// <summary>
    /// The UI language code (e.g., "en", "pl", "de").
    /// </summary>
    public string UI { get; set; } = "en";

    /// <summary>
    /// The content language code.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Text part language configurations.
    /// </summary>
    public List<TextPartLanguage>? TextPartLanguage { get; set; }
}
