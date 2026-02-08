using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 Editable Component.
/// Renders an editable area for CKEditor (useful for multiroot/decoupled editors).
/// </summary>
public partial class CKEditorEditable : ComponentBase
{
    /// <summary>
    /// The name of the root element in the editor.
    /// </summary>
    [Parameter]
    public string RootName { get; set; } = "main";

    /// <summary>
    /// The ID of the editor instance this editable belongs to.
    /// </summary>
    [Parameter]
    public string? EditorId { get; set; }

    /// <summary>
    /// The initial content value for the editable.
    /// </summary>
    [Parameter]
    public string? Content { get; set; }

    /// <summary>
    /// The debounce time in milliseconds for saving changes.
    /// </summary>
    [Parameter]
    public int SaveDebounceMs { get; set; } = 500;

    /// <summary>
    /// Optional name for the input field.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Whether the input is required.
    /// </summary>
    [Parameter]
    public bool? Required { get; set; }

    /// <summary>
    /// Optional CSS class for the editable container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Optional inline styles for the editable container.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Optional ID for the editable instance.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Optional CSS class for the inner content container.
    /// </summary>
    [Parameter]
    public string? InnerClass { get; set; }

    /// <summary>
    /// Optional inline styles for the inner content container.
    /// </summary>
    [Parameter]
    public string? InnerStyle { get; set; }

    private string StyleValue => $"position: relative;{(string.IsNullOrEmpty(Style) ? string.Empty : $" {Style}")}";

    protected override void OnInitialized()
    {
        Id ??= $"cke5-editable-{Guid.NewGuid():N}";
    }
}
