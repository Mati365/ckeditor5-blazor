using System.Text.Json;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components.Assets;

/// <summary>
/// Internal primitive that renders a <c>&lt;script type="importmap"&gt;</c> tag.
/// Emits nothing when the map is empty. Shared by <see cref="CKE5BundleRenderer"/>
/// and <see cref="CKE5Importmap"/>.
/// </summary>
public partial class CKE5ImportmapScript : ComponentBase
{
    /// <summary>
    /// The import map entries to render.
    /// </summary>
    [Parameter]
    public Dictionary<string, string> ImportMap { get; set; } = [];

    /// <summary>
    /// Optional nonce for CSP (Content Security Policy).
    /// </summary>
    [Parameter]
    public string? Nonce { get; set; }

    private string ImportMapJson => JsonSerializer.Serialize(new { imports = ImportMap });

    private Dictionary<string, object> GetNonceAttribute() =>
        string.IsNullOrEmpty(Nonce) ? [] : new Dictionary<string, object> { { "nonce", Nonce } };
}
