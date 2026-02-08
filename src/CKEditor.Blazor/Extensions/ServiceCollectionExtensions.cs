using CKEditor.Blazor.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CKEditor.Blazor.Extensions;

/// <summary>
/// Extension methods for registering CKEditor services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CKEditor services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCKEditor(this IServiceCollection services)
    {
        services.AddSingleton<ConfigManager>();

        return services;
    }
}
