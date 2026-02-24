namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents a text part language configuration.
/// </summary>
public sealed record TextPartLanguage
{
    /// <summary>
    /// The language code.
    /// </summary>
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// The title for this language option.
    /// </summary>
    public string? Title { get; init; }
}
