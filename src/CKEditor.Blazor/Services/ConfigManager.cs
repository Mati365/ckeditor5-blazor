using CKEditor.Blazor.Models;

namespace CKEditor.Blazor.Services;

/// <summary>
/// Service for managing CKEditor configurations, presets, and contexts.
/// </summary>
public class ConfigManager
{
    private readonly Dictionary<string, Preset> _presets = [];
    private readonly Dictionary<string, Context> _contexts = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigManager"/> class.
    /// </summary>
    public ConfigManager()
    {
        // Register default preset
        RegisterPreset("default", new Preset
        {
            EditorType = EditorType.Classic,
            Config = []
        });

        // Register default context
        RegisterContext("default", new Context
        {
            Config = []
        });
    }

    /// <summary>
    /// Registers a preset with the specified name.
    /// </summary>
    /// <param name="name">The name of the preset.</param>
    /// <param name="preset">The preset to register.</param>
    public void RegisterPreset(string name, Preset preset)
    {
        _presets[name] = preset;
    }

    /// <summary>
    /// Registers a context with the specified name.
    /// </summary>
    /// <param name="name">The name of the context.</param>
    /// <param name="context">The context to register.</param>
    public void RegisterContext(string name, Context context)
    {
        _contexts[name] = context;
    }

    /// <summary>
    /// Resolves a preset by name or returns the provided preset object.
    /// </summary>
    /// <param name="preset">The preset name or preset object.</param>
    /// <returns>The resolved preset.</returns>
    public Preset ResolvePreset(object? preset)
    {
        return preset switch
        {
            null => _presets.GetValueOrDefault("default") ?? new Preset(),
            string presetName => _presets.GetValueOrDefault(presetName) ?? throw new InvalidOperationException($"Unknown preset: {presetName}"),
            Preset presetObj => presetObj,
            _ => throw new ArgumentException("Invalid preset type", nameof(preset))
        };
    }

    /// <summary>
    /// Resolves a preset by name or throws an exception if not found.
    /// </summary>
    /// <param name="presetName">The name of the preset.</param>
    /// <returns>The resolved preset.</returns>
    public Preset ResolvePresetOrThrow(string presetName)
    {
        if (!_presets.TryGetValue(presetName, out var preset))
        {
            throw new InvalidOperationException($"Unknown preset: {presetName}");
        }

        return preset;
    }

    /// <summary>
    /// Resolves a context by name or returns the provided context object.
    /// </summary>
    /// <param name="context">The context name or context object.</param>
    /// <returns>The resolved context.</returns>
    public Context ResolveContext(object? context)
    {
        return context switch
        {
            null => _contexts.GetValueOrDefault("default") ?? new Context(),
            string contextName => _contexts.GetValueOrDefault(contextName) ?? throw new InvalidOperationException($"Unknown context: {contextName}"),
            Context contextObj => contextObj,
            _ => throw new ArgumentException("Invalid context type", nameof(context))
        };
    }

    /// <summary>
    /// Resolves a context by name or throws an exception if not found.
    /// </summary>
    /// <param name="contextName">The name of the context.</param>
    /// <returns>The resolved context.</returns>
    public Context ResolveContextOrThrow(string contextName)
    {
        if (!_contexts.TryGetValue(contextName, out var context))
        {
            throw new InvalidOperationException($"Unknown context: {contextName}");
        }

        return context;
    }

    /// <summary>
    /// Gets all registered presets.
    /// </summary>
    /// <returns>A read-only dictionary of all registered presets.</returns>
    public IReadOnlyDictionary<string, Preset> GetPresets() => _presets;

    /// <summary>
    /// Gets all registered contexts.
    /// </summary>
    /// <returns>A read-only dictionary of all registered contexts.</returns>
    public IReadOnlyDictionary<string, Context> GetContexts() => _contexts;
}
