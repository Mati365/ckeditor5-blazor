using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 Hidden Input Component.
/// Renders a hidden input field for form integration.
/// </summary>
public partial class CKEditorHiddenInput : ComponentBase
{
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
    /// Optional CSS class for the editor container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Optional inline styles for the editor container.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Optional ID for the editor instance.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    private string StyleValue => Style ?? GetDefaultStyles();

    protected override void OnInitialized()
    {
        Id ??= $"cke5-input-{Guid.NewGuid():N}";
    }

    private static string GetDefaultStyles()
    {
        var styles = new[]
        {
            "position: absolute",
            "left: 50%",
            "bottom: 0",
            "display: flex",
            "width: 1px",
            "height: 1px",
            "opacity: 0",
            "pointer-events: none",
            "margin: 0",
            "padding: 0",
            "border: none"
        };

        return string.Join("; ", styles) + ";";
    }
}
