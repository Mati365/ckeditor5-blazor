using System.Reflection;

namespace CKEditor.Blazor.Services;

/// <summary>
/// Provides CKEditor version information from assembly metadata.
/// </summary>
internal static class BuildMetadataReader
{
    private const string _ckeditorVersionKey = "CKEditor.Blazor.CKEditorVersion";
    private const string _ckboxVersionKey = "CKEditor.Blazor.CKBoxVersion";
    private const string _assetsOutputPathKey = "CKEditor.Blazor.AssetsOutputPath";
    private const string _includePremiumAssetsKey = "CKEditor.Blazor.IncludePremiumAssets";

    /// <summary>
    /// Resolves the CKEditor version from assembly metadata.
    /// Falls back to the default version if metadata is not found.
    /// </summary>
    /// <returns>The CKEditor version string.</returns>
    public static string ResolveCKEditorVersion() => ResolveValue(_ckeditorVersionKey) ?? "47.3.0";

    /// <summary>
    /// Resolves the CKBox version from assembly metadata.
    /// Falls back to the default version if metadata is not found.
    /// </summary>
    /// <returns>The CKBox version string.</returns>
    public static string ResolveCKBoxVersion() => ResolveValue(_ckboxVersionKey) ?? "2.8.0";

    /// <summary>
    /// Resolves the assets output path from assembly metadata.
    /// Falls back to the default path if metadata is not found.
    /// </summary>
    /// <returns>The assets output path string.</returns>
    public static string ResolveAssetsOutputPath() => ResolveValue(_assetsOutputPathKey) ?? "_content/ckeditor5";

    /// <summary>
    /// Resolves whether premium assets are included from assembly metadata.
    /// Falls back to false if metadata is not found.
    /// </summary>
    /// <returns>True if premium assets are included, false otherwise.</returns>
    public static bool ResolveIncludePremiumAssets()
    {
        var value = ResolveValue(_includePremiumAssetsKey);
        return !string.IsNullOrWhiteSpace(value) && bool.Parse(value!);
    }

    private static string? ResolveValue(string key)
    {
        // 1. Check Entry Assembly (the application)
        var entryValue = ResolveFromAssembly(Assembly.GetEntryAssembly(), key);
        if (!string.IsNullOrWhiteSpace(entryValue))
        {
            return entryValue;
        }

        // 2. Check Executing Assembly (this library)
        var executingValue = ResolveFromAssembly(Assembly.GetExecutingAssembly(), key);
        if (!string.IsNullOrWhiteSpace(executingValue))
        {
            return executingValue;
        }

        // 3. Check all loaded assemblies that might be relevant (e.g. referencing CKEditor.Blazor)
        // This is a heuristic to find configuration in intermediate libraries like RCLs.
        var relevantAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && a != Assembly.GetEntryAssembly() && a != Assembly.GetExecutingAssembly())
            .ToArray();

        foreach (var assembly in relevantAssemblies)
        {
            var value = ResolveFromAssembly(assembly, key);
            if (!string.IsNullOrWhiteSpace(value))
            {
                // Return the first one found. Ambiguity might be an issue if multiple libs define it,
                // but usually only one "provider" exists.
                return value;
            }
        }

        return null;
    }

    private static string? ResolveFromAssembly(Assembly? assembly, string key)
    {
        if (assembly is null)
        {
            return null;
        }

        return assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attr => string.Equals(attr.Key, key, StringComparison.Ordinal))?.Value;
    }
}
