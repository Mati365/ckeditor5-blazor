using CKEditor.Blazor.Model;

namespace CKEditor.Blazor.Serialization;

/// <summary>
/// Parser for <see cref="Language"/> values.
/// </summary>
public static class LanguageParser
{
    /// <summary>
    /// Parses a language configuration which can be: null, a language code string or a <see cref="Language"/> instance.
    /// </summary>
    /// <param name="language">Input language descriptor.</param>
    /// <returns>Parsed <see cref="Language"/> instance.</returns>
    public static Language Parse(object? language)
    {
        return language switch
        {
            null => new Language(),
            string code => new Language { UI = code, Content = code },
            Language lang => lang,
            _ => throw new ArgumentException("Invalid language type", nameof(language))
        };
    }
}
