using CKEditor.Blazor.Domain.Model;

namespace CKEditor.Blazor.Infrastructure;

/// <summary>
/// Extension methods for EditorType.
/// </summary>
public static class EditorTypeExtensions
{
    /// <summary>
    /// Determines if the editor type is Decoupled or Multiroot.
    /// </summary>
    /// <param name="editorType">The editor type to check.</param>
    /// <returns>True if the editor type is Decoupled or Multiroot; otherwise, false.</returns>
    public static bool IsDecoupledOrMultiroot(this EditorType editorType)
    {
        return editorType is EditorType.Multiroot or EditorType.Decoupled;
    }
}
