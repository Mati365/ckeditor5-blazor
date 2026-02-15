using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components.Assets;

/// <summary>
/// CKEditor 5 Integration Loader Component.
/// Automatically loads the ckeditor5-blazor JavaScript module.
/// </summary>
public partial class CKEditorIntegrationAssets : ComponentBase
{
    /// <summary>
    /// Optional nonce for CSP (Content Security Policy).
    /// </summary>
    [Parameter]
    public string? Nonce { get; set; }

    /// <summary>
    /// The URL of the module to load. Default is the static web asset path.
    /// </summary>
    [Parameter]
    public string ModuleUrl { get; set; } = "_content/CKEditor.Blazor/ckeditor5-blazor/index.mjs";

    private Dictionary<string, object> GetNonceAttribute()
    {
        if (string.IsNullOrEmpty(Nonce))
        {
            return [];
        }

        return new Dictionary<string, object> { { "nonce", Nonce } };
    }
}
