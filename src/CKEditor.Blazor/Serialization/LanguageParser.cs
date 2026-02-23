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

    /// <summary>
    /// Try-parse variant that does not throw on invalid input.
    /// </summary>
    /// <param name="language">Input language descriptor.</param>
    /// <param name="result">Parsed <see cref="Language"/> if successful; otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    public static bool TryParse(object? language, out Language? result)
    {
        try
        {
            result = Parse(language);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }
}
