using CKEditor.Blazor.Bundle;
using CKEditor.Blazor.Cloud;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components.Assets;

/// <summary>
/// CKEditor 5 Cloud Assets Component.
/// Renders the necessary script and stylesheet tags for CKEditor Cloud integration.
/// </summary>
public partial class CKEditorCloudAssets : ComponentBase
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
    /// Custom import map entries to merge with the generated import map.
    /// </summary>
    [Parameter]
    public Dictionary<string, string> CustomImportMap { get; set; } = [];

    [Inject]
    private ConfigManager ConfigManager { get; set; } = default!;

    private AssetsBundle? Bundle { get; set; }

    protected override void OnInitialized()
    {
        var preset = ConfigManager.ResolvePresetOrThrow(Preset);

        if (preset.Cloud == null)
        {
            return;
        }

        Bundle = CloudBundleBuilder.Build(preset.Cloud);
    }
}
