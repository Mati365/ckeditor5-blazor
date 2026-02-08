using System.Text.Json;
using CKEditor.Blazor.Preset;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 Context Component.
/// Renders a CKEditor context that can be shared among multiple editors.
/// </summary>
public partial class CKEditorContext : ComponentBase
{
    /// <summary>
    /// The language code for the context (default: 'en').
    /// </summary>
    [Parameter]
    public string? Language { get; set; } = "en";

    /// <summary>
    /// The context preset name or configuration object to use (default: 'default').
    /// </summary>
    [Parameter]
    public object? ContextPreset { get; set; } = null;

    /// <summary>
    /// Optional ID for the context instance.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    [Inject]
    private ConfigManager ConfigManager { get; set; } = default!;

    private string LanguageJson => JsonSerializer.Serialize(LanguageParser.Parse(Language));

    private string ContextJson => JsonSerializer.Serialize(ConfigManager.ResolveContext(ContextPreset ?? "default"));

    protected override void OnInitialized()
    {
        Id ??= $"cke5-context-{Guid.NewGuid():N}";
    }
}
