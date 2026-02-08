using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 UI Part Component.
/// Renders a UI part container (e.g., toolbar, menubar) for decoupled editors.
/// </summary>
public partial class CKEditorUIPart : ComponentBase
{
    /// <summary>
    /// The name of the UI part (e.g., "toolbar", "menubar").
    /// </summary>
    [Parameter]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the editor instance this UI part belongs to.
    /// </summary>
    [Parameter]
    public string? EditorId { get; set; }

    /// <summary>
    /// Optional CSS class for the UI part container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Optional inline styles for the UI part container.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Optional ID for the UI part instance.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    protected override void OnInitialized()
    {
        Id ??= $"cke5-ui-part-{Guid.NewGuid():N}";
    }
}
