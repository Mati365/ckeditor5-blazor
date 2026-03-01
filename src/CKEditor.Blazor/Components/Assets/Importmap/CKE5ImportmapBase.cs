using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components.Assets;

/// <summary>
/// Abstract base for importmap-only components (<see cref="CKE5CloudImportmap"/>,
/// <see cref="CKE5SelfHostedImportmap"/>). Holds all shared parameters and
/// the <see cref="ImportMap"/> computation; subclasses only need to provide
/// <see cref="BuildBundle"/>.
/// </summary>
public abstract class CKE5ImportmapBase : ComponentBase
{
    /// <summary>
    /// The preset name to use (default: 'default').
    /// </summary>
    [Parameter]
    public string Preset { get; set; } = "default";

    /// <summary>
    /// Optional nonce for CSP (Content Security Policy).
    /// </summary>
    [Parameter]
    public string? Nonce { get; set; }

    /// <summary>
    /// Custom import map entries to merge with the generated import map.
    /// </summary>
    [Parameter]
    public Dictionary<string, string> CustomImportMap { get; set; } = [];

    [Inject]
    protected ConfigManager ConfigManager { get; set; } = default!;

    /// <summary>
    /// The resolved import map, exposed as <c>protected</c> so Razor templates
    /// in derived partial classes can reference it.
    /// </summary>
    protected Dictionary<string, string> ImportMap
    {
        get
        {
            var importMap = Bundle?.GetImportMap() ?? [];

            foreach (var (key, value) in CustomImportMap)
            {
                importMap[key] = value;
            }

            return importMap;
        }
    }

    private AssetsBundle? Bundle { get; set; }

    protected override void OnInitialized()
    {
        var preset = ConfigManager.ResolvePresetOrThrow(Preset);
        Bundle = BuildBundle(preset);
    }

    /// <summary>
    /// Builds the <see cref="AssetsBundle"/> from the resolved preset.
    /// Implemented differently for cloud and self-hosted variants.
    /// </summary>
    /// <param name="preset">The resolved preset configuration.</param>
    /// <returns>The assets bundle containing the import map entries.</returns>
    protected abstract AssetsBundle? BuildBundle(PresetConfig preset);
}
