namespace CKEditor.Blazor.Components;

/// <summary>
/// Defines shared behavior for CKEditor components that can bootstrap in the browser
/// without .NET JS interop.
/// </summary>
public interface ICKE5InteractiveComponent
{
    /// <summary>
    /// When <see langword="true"/>, the component bootstraps itself via the JS Web Component
    /// without requiring Blazor .NET interop initialization.
    ///
    /// This is useful for pages rendered in static mode (non-interactive render mode),
    /// where .NET interop callbacks are not available.
    /// </summary>
    bool Interactive { get; set; }
}
