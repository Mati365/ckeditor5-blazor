using System.Text.Json;
using CKEditor.Blazor.Models;
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
    /// Whether to emit the import map script tag.
    /// </summary>
    [Parameter]
    public bool EmitImportMap { get; set; } = false;

    /// <summary>
    /// Custom import map entries to merge with the generated import map.
    /// </summary>
    [Parameter]
    public Dictionary<string, string> CustomImportMap { get; set; } = [];

    private List<JSAsset> EsmAssets { get; set; } = [];

    private List<JSAsset> UmdAssets { get; set; } = [];

    private List<string> CssUrls { get; set; } = [];

    private Dictionary<string, string> ImportMap { get; set; } = [];

    private string ImportMapJson => JsonSerializer.Serialize(ImportMap);

    protected override void OnInitialized()
    {
        // TODO: Load cloud configuration and build bundle
        LoadCloudAssets();
    }

    private void LoadCloudAssets()
    {
        // TODO: Implement proper cloud asset loading logic
        // This should load from configuration and build the asset bundle
        var generatedImportMap = new Dictionary<string, string>();

        // Group JS assets by type
        foreach (var asset in EsmAssets)
        {
            generatedImportMap[asset.Name] = asset.Url;
        }

        // Merge with custom import map
        ImportMap = generatedImportMap
            .Concat(CustomImportMap)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
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
