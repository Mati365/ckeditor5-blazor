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
    /// Gets or sets the dictionary of context configurations.
    /// The key is the context name and the value is the ContextConfig object.
    /// </summary>
    public Dictionary<string, ContextConfig> Contexts { get; set; } = [];

    /// <summary>
    /// Sets <see cref="DefaultLicenseKey"/> and returns <see langword="this"/> for fluent chaining.
    /// </summary>
    /// <param name="licenseKey">The raw license key string (e.g. <c>"GPL"</c> or a JWT token).</param>
    /// <returns>The current <see cref="CKEditorOptions"/> instance.</returns>
    public CKEditorOptions SetLicenseKey(string licenseKey)
    {
        DefaultLicenseKey = licenseKey;
        return this;
    }

    /// <summary>
    /// Registers a preset <paramref name="name"/> and returns <see langword="this"/> for fluent chaining.
    /// </summary>
    /// <param name="name">The preset name used to reference it from Razor components.</param>
    /// <param name="preset">The preset configuration to register.</param>
    /// <returns>The current <see cref="CKEditorOptions"/> instance.</returns>
    public CKEditorOptions AddPreset(string name, PresetConfig preset)
    {
        Presets[name] = preset;
        return this;
    }

    /// <summary>
    /// Builds and registers a preset under <paramref name="name"/> by applying
    /// <paramref name="configure"/> to a blank <see cref="PresetConfig"/>.
    /// Returns <see langword="this"/> for fluent chaining.
    /// </summary>
    /// <param name="name">The preset name used to reference it from Razor components.</param>
    /// <param name="configure">A function that transforms a blank preset into the desired configuration.</param>
    /// <returns>The current <see cref="CKEditorOptions"/> instance.</returns>
    public CKEditorOptions AddPreset(string name, Func<PresetConfig, PresetConfig> configure)
    {
        Presets[name] = configure(new PresetConfig());
        return this;
    }

    /// <summary>
    /// Registers <paramref name="preset"/> as the <c>"default"</c> preset and returns
    /// <see langword="this"/> for fluent chaining.
    /// </summary>
    /// <param name="preset">The preset configuration to use as the default.</param>
    /// <returns>The current <see cref="CKEditorOptions"/> instance.</returns>
    public CKEditorOptions AddDefaultPreset(PresetConfig preset) => AddPreset("default", preset);

    /// <summary>
    /// Builds and registers the <c>"default"</c> preset by applying <paramref name="configure"/>
    /// to a blank <see cref="PresetConfig"/>.
    /// Returns <see langword="this"/> for fluent chaining.
    /// </summary>
    /// <param name="configure">A function that transforms a blank preset into the desired configuration.</param>
    /// <returns>The current <see cref="CKEditorOptions"/> instance.</returns>
    public CKEditorOptions AddDefaultPreset(Func<PresetConfig, PresetConfig> configure) => AddPreset("default", configure);

    /// <summary>
    /// Builds and registers the <c>"default"</c> preset by applying <paramref name="configure"/>
    /// to the base default preset configuration.
    /// Returns <see langword="this"/> for fluent chaining.
    /// </summary>
    /// <param name="configure">A function that transforms the default preset into the desired configuration.</param>
    /// <returns>The current <see cref="CKEditorOptions"/> instance.</returns>
    public CKEditorOptions ExtendDefaultPreset(Func<PresetConfig, PresetConfig> configure)
    {
        var defaultPreset = ConfigManager.CreateDefaultPreset(GetParsedLicenseKey());
        Presets["default"] = configure(defaultPreset);
        return this;
    }

    /// <summary>
    /// Registers a context <paramref name="name"/> and returns <see langword="this"/> for fluent chaining.
    /// </summary>
    /// <param name="name">The context name used to reference it from Razor components.</param>
    /// <param name="context">The context configuration to register.</param>
    /// <returns>The current <see cref="CKEditorOptions"/> instance.</returns>
    public CKEditorOptions AddContext(string name, ContextConfig context)
    {
        Contexts[name] = context;
        return this;
    }

    /// <summary>
    /// Builds and registers a context under <paramref name="name"/> by applying
    /// <paramref name="configure"/> to a blank <see cref="ContextConfig"/>.
    /// Returns <see langword="this"/> for fluent chaining.
    /// </summary>
    /// <param name="name">The context name used to reference it from Razor components.</param>
    /// <param name="configure">A function that transforms a blank context into the desired configuration.</param>
    /// <returns>The current <see cref="CKEditorOptions"/> instance.</returns>
    public CKEditorOptions AddContext(string name, Func<ContextConfig, ContextConfig> configure)
    {
        Contexts[name] = configure(new ContextConfig());
        return this;
    }

    /// <summary>
    /// Registers <paramref name="context"/> as the <c>"default"</c> context and returns
    /// <see langword="this"/> for fluent chaining.
    /// </summary>
    /// <param name="context">The context configuration to use as the default.</param>
    /// <returns>The current <see cref="CKEditorOptions"/> instance.</returns>
    public CKEditorOptions AddDefaultContext(ContextConfig context) => AddContext("default", context);

    /// <summary>
    /// Builds and registers the <c>"default"</c> context by applying <paramref name="configure"/>
    /// to a blank <see cref="ContextConfig"/>.
    /// Returns <see langword="this"/> for fluent chaining.
    /// </summary>
    /// <param name="configure">A function that transforms a blank context into the desired configuration.</param>
    /// <returns>The current <see cref="CKEditorOptions"/> instance.</returns>
    public CKEditorOptions AddDefaultContext(Func<ContextConfig, ContextConfig> configure) => AddContext("default", configure);

    /// <summary>
    /// Builds and registers the <c>"default"</c> context by applying <paramref name="configure"/>
    /// to the base default context configuration.
    /// Returns <see langword="this"/> for fluent chaining.
    /// </summary>
    /// <param name="configure">A function that transforms the default context into the desired configuration.</param>
    /// <returns>The current <see cref="CKEditorOptions"/> instance.</returns>
    public CKEditorOptions ExtendDefaultContext(Func<ContextConfig, ContextConfig> configure)
    {
        var defaultContext = ConfigManager.CreateDefaultContext();
        Contexts["default"] = configure(defaultContext);
        return this;
    }

    /// <summary>
    /// Gets the parsed default license key, or null if not set or invalid.
    /// </summary>
    /// <returns>The parsed LicenseKey instance or null.</returns>
    public LicenseKey? GetParsedLicenseKey() => LicenseKeyParser.TryParse(DefaultLicenseKey, out var key) ? key : null;
}
