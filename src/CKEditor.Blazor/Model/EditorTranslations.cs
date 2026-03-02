namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents custom translations for the editor.
/// The outer key is a language code (e.g., <c>"en"</c>, <c>"pl"</c>),
/// and the inner dictionary maps original UI strings to their translated equivalents.
/// </summary>
public sealed class EditorTranslations : Dictionary<string, Dictionary<string, string>>
{
}
