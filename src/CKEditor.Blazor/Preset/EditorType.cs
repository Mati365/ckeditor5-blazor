using System.Text.Json;
using System.Text.Json.Serialization;

namespace CKEditor.Blazor.Preset;

/// <summary>
/// Represents the type of CKEditor instance.
/// </summary>
[JsonConverter(typeof(EditorTypeJsonConverter))]
public sealed class EditorType(string value)
{
    /// <summary>
    /// Classic editor with toolbar above the editing area.
    /// </summary>
    public static readonly EditorType Classic = new("classic");

    /// <summary>
    /// Inline editor that activates on click.
    /// </summary>
    public static readonly EditorType Inline = new("inline");

    /// <summary>
    /// Balloon editor with a floating toolbar.
    /// </summary>
    public static readonly EditorType Balloon = new("balloon");

    /// <summary>
    /// Decoupled editor with separate toolbar and editable areas.
    /// </summary>
    public static readonly EditorType Decoupled = new("decoupled");

    /// <summary>
    /// Multiroot editor with multiple editable areas.
    /// </summary>
    public static readonly EditorType Multiroot = new("multiroot");

    /// <summary>
    /// The string value of the editor type.
    /// </summary>
    public string Value => value;

    /// <summary>
    /// Implicitly converts an EditorType to a string.
    /// </summary>
    /// <param name="editorType">The editor type to convert.</param>
    /// <returns>The string value of the editor type.</returns>
    public static implicit operator string(EditorType editorType)
    {
        return editorType.Value;
    }

    /// <summary>
    /// Determines whether two editor types are equal.
    /// </summary>
    /// <param name="left">The first editor type to compare.</param>
    /// <param name="right">The second editor type to compare.</param>
    /// <returns>True if the editor types are equal; otherwise, false.</returns>
    public static bool operator ==(EditorType? left, EditorType? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Determines whether two editor types are not equal.
    /// </summary>
    /// <param name="left">The first editor type to compare.</param>
    /// <param name="right">The second editor type to compare.</param>
    /// <returns>True if the editor types are not equal; otherwise, false.</returns
    public static bool operator !=(EditorType? left, EditorType? right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// Parses a string value into an EditorType.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <returns>The corresponding EditorType.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not a valid editor type.</exception>
    public static EditorType Parse(string value)
    {
        return value?.ToLowerInvariant() switch
        {
            "classic" => Classic,
            "inline" => Inline,
            "balloon" => Balloon,
            "decoupled" => Decoupled,
            "multiroot" => Multiroot,
            _ => throw new ArgumentException($"Unknown editor type: {value}", nameof(value))
        };
    }

    /// <summary>
    /// Determines if the editor type is Decoupled or Multiroot.
    /// </summary>
    /// <returns>True if the editor type is Decoupled or Multiroot; otherwise, false.</returns>
    public bool IsDecoupledOrMultiroot()
    {
        return this == Multiroot || this == Decoupled;
    }

    /// <summary>
    /// Returns the string representation of the editor type.
    /// </summary>
    /// <returns>The string value of the editor type.</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current editor type.
    /// </summary>
    /// <param name="obj">The object to compare with the current editor type.</param>
    /// <returns>True if the specified object is equal to the current editor type; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is EditorType other && Value == other.Value;
    }

    /// <summary>
    /// Returns the hash code for this editor type.
    /// </summary>
    /// <returns>A hash code for the current editor type.</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
