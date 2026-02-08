using System.Text.Json;
using CKEditor.Blazor.Models;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor;

/// <summary>
/// CKEditor 5 Main Component.
/// Renders a CKEditor instance with configurable options.
/// </summary>
public partial class CKEditor5 : ComponentBase
{
    /// <summary>
    /// The initial content of the editor. Can be a string or a dictionary for multiroot editors.
    /// </summary>
    [Parameter]
    public object? Content { get; set; }

    /// <summary>
    /// The preset name or object to use (default: 'default').
    /// </summary>
    [Parameter]
    public object? Preset { get; set; }

    /// <summary>
    /// Whether to enable the watchdog feature (default: true).
    /// </summary>
    [Parameter]
    public bool Watchdog { get; set; } = true;

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
    /// Optional height for the editable area in pixels.
    /// </summary>
    [Parameter]
    public int? EditableHeight { get; set; }

    /// <summary>
    /// Optional CSS class for the editor container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Optional inline styles for the editor container.
    /// </summary>
    [Parameter]
    public string? Style { get; set; } = "display: block; width: 100%;";

    /// <summary>
    /// Optional ID for the editor instance.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Optional context ID for multiple editors sharing a context.
    /// </summary>
    [Parameter]
    public string? ContextId { get; set; }

    /// <summary>
    /// Optional language configuration (string or object).
    /// </summary>
    [Parameter]
    public object? Language { get; set; }

    /// <summary>
    /// Optional editor configuration overrides (shallow replace).
    /// </summary>
    [Parameter]
    public Dictionary<string, object>? Config { get; set; }

    /// <summary>
    /// Optional editor configuration to merge (deep merge).
    /// </summary>
    [Parameter]
    public Dictionary<string, object>? MergeConfig { get; set; }

    /// <summary>
    /// Optional custom translations dictionary.
    /// </summary>
    [Parameter]
    public Dictionary<string, string>? CustomTranslations { get; set; }

    /// <summary>
    /// Optional editor type to use (e.g., "classic", "inline", "balloon", "decoupled", "multiroot").
    /// </summary>
    [Parameter]
    public string? EditorType { get; set; }

    [Inject]
    private ConfigManager ConfigManager { get; set; } = default!;

    private string StyleValue => $"position: relative;{(string.IsNullOrEmpty(Style) ? string.Empty : $" {Style}")}";

    private string PresetJson => JsonSerializer.Serialize(ResolvePreset());

    private string ContentJson => JsonSerializer.Serialize(NormalizeContent());

    private string LanguageJson => JsonSerializer.Serialize(ParseLanguage());

    private bool ShowInput => !IsDecoupledOrMultiroot();

    protected override void OnInitialized()
    {
        Id ??= $"cke5-{Guid.NewGuid():N}";
    }

    private Preset ResolvePreset()
    {
        var preset = ConfigManager.ResolvePreset(Preset ?? "default");

        preset = ApplyConfigOverride(preset);
        preset = ApplyConfigMerge(preset);
        preset = ApplyCustomTranslations(preset);
        preset = ApplyEditorTypeOverride(preset);

        return preset;
    }

    private Preset ApplyConfigOverride(Preset preset)
    {
        return Config == null ? preset : preset.OfConfig(Config);
    }

    private Preset ApplyConfigMerge(Preset preset)
    {
        return MergeConfig == null ? preset : preset.OfMergedConfig(MergeConfig);
    }

    private Preset ApplyCustomTranslations(Preset preset)
    {
        return CustomTranslations == null ? preset : preset.OfCustomTranslations(CustomTranslations);
    }

    private Preset ApplyEditorTypeOverride(Preset preset)
    {
        if (string.IsNullOrWhiteSpace(EditorType))
        {
            return preset;
        }

        return Enum.TryParse<EditorType>(EditorType, true, out var editorType)
            ? preset.OfEditorType(editorType)
            : preset;
    }

    private object NormalizeContent()
    {
        return Content switch
        {
            string stringContent => new Dictionary<string, string> { { "main", stringContent } },
            null => new Dictionary<string, string>(),
            _ => Content
        };
    }

    private Language ParseLanguage()
    {
        return LanguageParser.Parse(Language);
    }

    private bool IsDecoupledOrMultiroot()
    {
        var preset = ResolvePreset();
        return preset.EditorType is Models.EditorType.Multiroot or Models.EditorType.Decoupled;
    }

    private Dictionary<string, object> GetAdditionalAttributes()
    {
        var attributes = new Dictionary<string, object>();

        if (Watchdog)
        {
            attributes["data-cke-watchdog"] = "true";
        }

        if (!string.IsNullOrEmpty(ContextId))
        {
            attributes["data-cke-context-id"] = ContextId;
        }

        if (EditableHeight.HasValue)
        {
            attributes["data-cke-editable-height"] = EditableHeight.Value;
        }

        return attributes;
    }
}
