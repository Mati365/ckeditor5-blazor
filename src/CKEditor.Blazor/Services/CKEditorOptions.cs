using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.License;
using CKEditor.Blazor.Serialization;

namespace CKEditor.Blazor.Services;

/// <summary>
/// Options for configuring CKEditor services.
/// </summary>
public class CKEditorOptions
{
    /// <summary>
    /// Gets or sets the default license key for all presets.
    /// This can be set in appsettings.json, environment variables, or programmatically.
    /// </summary>
    public string? DefaultLicenseKey { get; set; }

    /// <summary>
    /// Gets or sets the dictionary of preset configurations.
    /// The key is the preset name and the value is the PresetConfig object.
    /// </summary>
    public Dictionary<string, PresetConfig> Presets { get; set; } = [];

    /// <summary>
    /// Gets the parsed default license key, or null if not set or invalid.
    /// </summary>
    /// <returns>The parsed LicenseKey instance or null.</returns>
    internal LicenseKey? GetParsedLicenseKey() => LicenseKeyParser.TryParse(DefaultLicenseKey, out var key) ? key : null;
}
