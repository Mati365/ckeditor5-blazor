using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components;

/// <summary>
/// CKEditor 5 Bootstrap Components. It should be used when non-interactive
/// page contains CKEditor components to ensure that the necessary JavaScript is loaded.
/// If you are using CKEditor components in an interactive page, this should not be used, as the main
/// CKEditor5 component will automatically load the necessary JavaScript on demand.
/// </summary>
public partial class CKEditor5BootstrapComponents : ComponentBase
{
    /// <summary>
    /// Optional nonce for CSP (Content Security Policy).
    /// </summary>
    [Parameter]
    public string? Nonce { get; set; }

    private Dictionary<string, object> GetNonceAttribute()
    {
        if (string.IsNullOrEmpty(Nonce))
        {
            return [];
        }

        return new Dictionary<string, object> { { "nonce", Nonce } };
    }
}
