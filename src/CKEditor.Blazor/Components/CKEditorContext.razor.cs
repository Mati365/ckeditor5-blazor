using System.Text.Json;
using System.Text.Json.Serialization;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 Context Component.
/// Renders a CKEditor context that can be shared among multiple editors.
/// </summary>
public partial class CKEditorContext : ComponentBase
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// The language code for the context (default: 'en').
    /// </summary>
    [Parameter]
    public string? Language { get; set; } = "en";

    /// <summary>
    /// The context preset name or configuration object to use (default: 'default').
    /// </summary>
    [Parameter]
    public object? ContextPreset { get; set; }

    /// <summary>
    /// Optional child content to render inside the context component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional ID for the context instance.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    [Inject]
    private ConfigManager ConfigManager { get; set; } = default!;

    private string LanguageJson => JsonSerializer.Serialize(Preset.Language.Parse(Language), _jsonOptions);

    private string ContextJson => JsonSerializer.Serialize(ConfigManager.ResolveContext(ContextPreset ?? "default"), _jsonOptions);

    protected override void OnInitialized()
    {
        Id ??= $"cke5-context-{Guid.NewGuid():N}";
    }
}
