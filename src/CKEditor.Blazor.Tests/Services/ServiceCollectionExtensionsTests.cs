using CKEditor.Blazor.Services;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;
using CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;
using CKEditor.Blazor.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CKEditor.Blazor.Tests.Services;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCKEditor_WithoutAction_DoesNotThrow()
    {
        var services = new ServiceCollection();

        var exception = Record.Exception(() => services.AddCKEditor());

        Assert.Null(exception);

        var provider = services.BuildServiceProvider();
        var options = provider.GetService<IOptions<CKEditorOptions>>();

        Assert.NotNull(options);
        Assert.Null(options.Value.DefaultLicenseKey);
    }

    [Fact]
    public void AddCKEditor_WithAction_RegistersRequiredServices()
    {
        var services = new ServiceCollection();

        services.AddCKEditor(options => options.DefaultLicenseKey = "GPL");

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<ConfigManager>());
        Assert.NotNull(provider.GetService<ICKEditorCloudBundleBuilder>());
        Assert.NotNull(provider.GetService<ICKEditorPremiumCloudBundleBuilder>());
        Assert.NotNull(provider.GetService<ICKBoxCloudBundleBuilder>());
        Assert.NotNull(provider.GetService<ICloudBundleBuilder>());
        Assert.NotNull(provider.GetService<ICKEditorSelfHostedBundleBuilder>());
        Assert.NotNull(provider.GetService<ICKEditorPremiumSelfHostedBundleBuilder>());
        Assert.NotNull(provider.GetService<ICKBoxSelfHostedBundleBuilder>());
        Assert.NotNull(provider.GetService<ISelfHostedBundleBuilder>());

        var options = provider.GetService<IOptions<CKEditorOptions>>();
        Assert.NotNull(options);
        Assert.Equal("GPL", options.Value.DefaultLicenseKey);
    }

    [Fact]
    public void AddCKEditor_WithIConfiguration_RegistersRequiredServices()
    {
        var jwt = JwtTestHelper.BuildValid("sh");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DefaultLicenseKey"] = jwt,
            })
            .Build();

        var services = new ServiceCollection();

        services.AddCKEditor(configuration);

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<ConfigManager>());

        var options = provider.GetService<IOptions<CKEditorOptions>>();

        Assert.NotNull(options);
        Assert.Equal(jwt, options.Value.DefaultLicenseKey);
    }
}
