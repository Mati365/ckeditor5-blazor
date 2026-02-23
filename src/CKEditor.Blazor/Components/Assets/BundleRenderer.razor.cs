using System.Text.Json;
using CKEditor.Blazor.Model.Bundle;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components.Assets;

/// <summary>
/// Internal component for rendering CKEditor bundle assets (scripts, stylesheets, import maps).
/// </summary>
public partial class BundleRenderer : ComponentBase
{
    /// <summary>
    /// Optional nonce for CSP (Content Security Policy).
    /// </summary>
    [Parameter]
    public string? Nonce { get; set; }

    /// <summary>
    /// Whether to emit the import map script tag. Default is true.
    /// </summary>
    [Parameter]
    public bool EmitImportMap { get; set; } = true;

    /// <summary>
    /// The assets bundle to render.
    /// </summary>
    [Parameter]
    public AssetsBundle? Bundle { get; set; }

    /// <summary>
    /// Custom import map entries to merge with the generated import map.
    /// </summary>
    [Parameter]
    public Dictionary<string, string> CustomImportMap { get; set; } = [];

    private List<string> EsmAssets => Bundle?.GetEsmOnlyUrls() ?? [];

    private List<string> UmdAssets => Bundle?.GetUmdUrls() ?? [];

    private List<string> CssUrls => Bundle?.GetCssUrls() ?? [];

    private Dictionary<string, string> ImportMap
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

    private string ImportMapJson => JsonSerializer.Serialize(new { imports = ImportMap });

    private Dictionary<string, object> GetNonceAttribute()
    {
        if (string.IsNullOrEmpty(Nonce))
        {
            return [];
        }

        return new Dictionary<string, object> { { "nonce", Nonce } };
    }
}
