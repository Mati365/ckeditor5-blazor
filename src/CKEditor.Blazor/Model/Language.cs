namespace CKEditor.Blazor.Model;

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
    public string Content { get; set; } = "en";

    /// <summary>
    /// Text part language configurations.
    /// </summary>
    public List<TextPartLanguage>? TextPartLanguage { get; set; }

    /// <summary>
    /// Parses a language configuration from a string or object.
    /// </summary>
    /// <param name="language">The language configuration. Can be null (defaults to "en"), a language code string, or a Language object.</param>
    /// <returns>A Language object parsed from the input.</returns>
    public static Language Parse(object? language)
    {
        return language switch
        {
            null => new Language(),
            string languageCode => new Language { UI = languageCode, Content = languageCode },
            Language languageObj => languageObj,
            _ => throw new ArgumentException("Invalid language type", nameof(language))
        };
    }
}
