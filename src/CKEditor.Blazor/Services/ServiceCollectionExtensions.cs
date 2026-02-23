using CKEditor.Blazor.Services.Bundle.Cloud;
using CKEditor.Blazor.Services.Bundle.SelfHosted;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;
using CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CKEditor.Blazor.Services;

/// <summary>
/// Extension methods for registering CKEditor services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CKEditor services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configureOptions">An optional action to configure CKEditor options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCKEditor(
        this IServiceCollection services,
        Action<CKEditorOptions>? configureOptions = null)
    {
        services.Configure<CKEditorOptions>(options => configureOptions?.Invoke(options));

        services.AddSingleton<ConfigManager>();

        services.AddSingleton<ICKEditorCloudBundleBuilder, CKEditorCloudBundleBuilder>();
        services.AddSingleton<ICKEditorPremiumCloudBundleBuilder, CKEditorPremiumCloudBundleBuilder>();
        services.AddSingleton<ICKBoxCloudBundleBuilder, CKBoxCloudBundleBuilder>();
        services.AddSingleton<ICloudBundleBuilder, CloudBundleBuilder>();

        services.AddSingleton<ICKEditorSelfHostedBundleBuilder, CKEditorSelfHostedBundleBuilder>();
        services.AddSingleton<ICKEditorPremiumSelfHostedBundleBuilder, CKEditorPremiumSelfHostedBundleBuilder>();
        services.AddSingleton<ICKBoxSelfHostedBundleBuilder, CKBoxSelfHostedBundleBuilder>();
        services.AddSingleton<ISelfHostedBundleBuilder, SelfHostedBundleBuilder>();

        return services;
    }

    /// <summary>
    /// Adds CKEditor services to the specified <see cref="IServiceCollection"/> with configuration binding.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration section to bind.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCKEditor(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CKEditorOptions>(configuration);
        services.AddSingleton<ConfigManager>();

        return services;
    }
}
