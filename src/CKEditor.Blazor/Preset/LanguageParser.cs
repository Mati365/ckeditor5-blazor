namespace CKEditor.Blazor.Preset;

/// <summary>
/// Parser for language configurations.
/// </summary>
public static class LanguageParser
{
    /// <summary>
    /// Parses a language configuration from a string or object.
    /// </summary>
    /// <param name="language">The language configuration. Can be null (defaults to "en"), a language code string, or a Language object.</param>
    /// <returns>A Language object parsed from the input.</returns>
    public static Language Parse(object? language)
    {
        return language switch
        {
            null => new Language { UI = "en" },
            string languageCode => new Language { UI = languageCode },
            Language languageObj => languageObj,
            _ => throw new ArgumentException("Invalid language type", nameof(language))
        };
    }
}
