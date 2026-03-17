namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents additional HTML attributes to be applied to an editor root element.
/// This allows users to specify custom attributes (e.g., data attributes, ARIA attributes)
/// that will be rendered on the root element of a CKEditor editable region.
/// </summary>
public sealed class EditorRootAttributes : Dictionary<string, object?>
{
}
