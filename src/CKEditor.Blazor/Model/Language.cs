namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents language configuration for CKEditor.
/// </summary>
public sealed record Language
{
    /// <summary>
    /// The UI language code (e.g., "en", "pl", "de").
    /// </summary>
    public string UI { get; init; } = "en";

    /// <summary>
    /// The content language code.
    /// </summary>
    public string Content { get; init; } = "en";

    /// <summary>
    /// Text part language configurations.
    /// </summary>
    public List<TextPartLanguage>? TextPartLanguage { get; init; }
}
