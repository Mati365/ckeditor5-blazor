namespace CKEditor.Blazor.Preset;

/// <summary>
/// Utility methods for working with EditorType enum values.
/// </summary>
public static class EditorTypeExtensions
{
    /// <summary>
    /// Parses a string to an EditorType enum value.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <returns>The parsed EditorType value.</returns>
    /// <exception cref="ArgumentException">Thrown when the value cannot be parsed to EditorType.</exception>
    public static EditorType Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Editor type value cannot be null or empty.", nameof(value));
        }

        if (!Enum.TryParse<EditorType>(value, true, out var editorType))
        {
            throw new ArgumentException($"Invalid editor type: {value}. Valid values are: {string.Join(", ", Enum.GetNames<EditorType>())}.", nameof(value));
        }

        return editorType;
    }

    /// <summary>
    /// Determines if the given EditorType is Decoupled or Multiroot.
    /// </summary>
    /// <param name="editorType">The EditorType to check.</param>
    /// <returns>True if the editor type is Decoupled or Multiroot; otherwise, false.</returns>
    public static bool IsDecoupledOrMultiroot(EditorType editorType)
    {
        return editorType == EditorType.Multiroot || editorType == EditorType.Decoupled;
    }
}
