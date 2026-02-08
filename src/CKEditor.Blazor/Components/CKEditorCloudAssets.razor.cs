using System.Text.Json;
using CKEditor.Blazor.Cloud;
using CKEditor.Blazor.Cloud.Bundle;
using CKEditor.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components;

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

    private List<JSAsset> EsmAssets { get; set; } = [];

    private List<JSAsset> UmdAssets { get; set; } = [];

    private List<string> CssUrls { get; set; } = [];

    private Dictionary<string, string> ImportMap { get; set; } = [];

    private string ImportMapJson => JsonSerializer.Serialize(new { imports = ImportMap });

    protected override void OnInitialized()
    {
        var preset = ConfigManager.ResolvePresetOrThrow(Preset);

        if (preset.Cloud == null)
        {
            return;
        }

        LoadCloudAssets(preset.Cloud);
    }

    private void LoadCloudAssets(CloudConfig cloud)
    {
        var bundle = CloudBundleBuilder.Build(cloud);

        EsmAssets = [.. bundle.Js.Where(asset => asset.Type == JSAssetType.ESM)];
        UmdAssets = [.. bundle.Js.Where(asset => asset.Type == JSAssetType.UMD)];
        CssUrls = [.. bundle.Css.Distinct(StringComparer.OrdinalIgnoreCase)];

        var generatedImportMap = new Dictionary<string, string>();

        // Group JS assets by type
        foreach (var asset in EsmAssets)
        {
            generatedImportMap[asset.Name] = asset.Url;
        }

        // Merge with custom import map
        ImportMap = new Dictionary<string, string>(generatedImportMap);

        foreach (var (key, value) in CustomImportMap)
        {
            ImportMap[key] = value;
        }
    }

    private Dictionary<string, object> GetNonceAttribute()
    {
        if (string.IsNullOrEmpty(Nonce))
        {
            return [];
        }

        return new Dictionary<string, object> { { "nonce", Nonce } };
    }
}
