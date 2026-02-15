using System.Reflection;

namespace CKEditor.Blazor.Services;

/// <summary>
/// Provides CKEditor version information from assembly metadata.
/// </summary>
internal static class BuildMetadataReader
{
    private const string _ckeditorVersionKey = "CKEditor.Blazor.CKEditorVersion";
    private const string _defaultVersion = "47.3.0";

    /// <summary>
    /// Resolves the CKEditor version from assembly metadata.
    /// Falls back to the default version if metadata is not found.
    /// </summary>
    /// <returns>The CKEditor version string.</returns>
    public static string ResolveCKEditorVersion()
    {
        var entryAssemblyVersion = ResolveFromAssembly(Assembly.GetEntryAssembly());

        if (!string.IsNullOrWhiteSpace(entryAssemblyVersion))
        {
            return entryAssemblyVersion;
        }

        var executingAssemblyVersion = ResolveFromAssembly(Assembly.GetExecutingAssembly());

        if (!string.IsNullOrWhiteSpace(executingAssemblyVersion))
        {
            return executingAssemblyVersion;
        }

        return _defaultVersion;
    }

    private static string? ResolveFromAssembly(Assembly? assembly)
    {
        if (assembly is null)
        {
            return null;
        }

        foreach (var metadata in assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
        {
            if (string.Equals(metadata.Key, _ckeditorVersionKey, StringComparison.Ordinal))
            {
                return metadata.Value;
            }
        }

        return null;
    }
}
