using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components.Assets;

/// <summary>
/// CKEditor 5 Cloud Assets Component.
/// Renders the necessary script and stylesheet tags for CKEditor Cloud integration.
/// </summary>
public partial class CKE5Cloud : ComponentBase
{
    /// <summary>
    /// The preset name to use (default: 'default').
    /// Such preset should contain cloud configuration.
    /// </summary>
    [Parameter]
    public string Preset { get; set; } = "default";

    /// <summary>
    /// Optional nonce for CSP (Content Security Policy).
    /// </summary>
    [Parameter]
    public string? Nonce { get; set; }

    /// <summary>
    /// Whether to emit the import map script tag. Default is true.
    /// Set to false if you want to manage the import map yourself.
    /// </summary>
    [Parameter]
    public bool EmitImportMap { get; set; } = true;

    /// <summary>
    /// Whether to emit module preload link tags for ESM assets. Default is true.
    /// Set to false when the import map is declared globally (e.g. via <c>&lt;CKE5CloudImportmap /&gt;</c>)
    /// and you only want per-page assets such as stylesheets rendered here.
    /// </summary>
    [Parameter]
    public bool EmitModulePreload { get; set; } = true;

    /// <summary>
    /// Custom import map entries to merge with the generated import map.
    /// </summary>
    [Parameter]
    public Dictionary<string, string> CustomImportMap { get; set; } = [];

    /// <summary>
    /// Optional injection targets and stored state for the component. All
    /// members are protected so that the style rules (protected before private)
    /// are satisfied even when they appear above lifecycle methods.
    /// </summary>
    [Inject]
    protected ConfigManager ConfigManager { get; set; } = default!;

    [Inject]
    protected ICloudBundleBuilder CloudBundleBuilder { get; set; } = default!;

    protected AssetsBundle? Bundle { get; set; }

    protected override void OnInitialized()
    {
        var preset = ConfigManager.ResolvePresetOrThrow(Preset);
        var cloud = preset.EnsureCloudCompatibilityOrThrow();

        Bundle = CloudBundleBuilder.Build(cloud);
    }
}
