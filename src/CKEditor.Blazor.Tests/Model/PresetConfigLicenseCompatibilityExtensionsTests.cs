using CKEditor.Blazor.Exceptions;
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Cloud;
using CKEditor.Blazor.Model.License;

namespace CKEditor.Blazor.Tests.Model;

public class PresetConfigLicenseCompatibilityExtensionsTests
{
    [Fact]
    public void EnsureCloudCompatibilityOrThrow_NoCloud_ThrowsCloudConfigurationMissingException()
    {
        var preset = new PresetConfig { Cloud = null };

        Assert.Throws<CloudConfigurationMissingException>(() => preset.EnsureCloudCompatibilityOrThrow());
    }

    [Fact]
    public void EnsureCloudCompatibilityOrThrow_CustomCdnWithCloudOnlyLicense_ThrowsCloudLicenseIncompatibleException()
    {
        var preset = new PresetConfig
        {
            Cloud = new CloudConfig { CdnUrl = "https://custom.cdn.com" },
            LicenseKey = new LicenseKey("raw", DistributionChannel.Cloud)
        };

        Assert.Throws<CloudLicenseIncompatibleException>(() => preset.EnsureCloudCompatibilityOrThrow());
    }

    [Fact]
    public void EnsureCloudCompatibilityOrThrow_OfficialCdnWithIncompatibleLicense_ThrowsCloudLicenseIncompatibleException()
    {
        var preset = new PresetConfig
        {
            Cloud = new CloudConfig { CdnUrl = "https://cdn.ckeditor.com" },
            LicenseKey = new LicenseKey("raw", DistributionChannel.SH)
        };

        Assert.Throws<CloudLicenseIncompatibleException>(() => preset.EnsureCloudCompatibilityOrThrow());
    }

    [Fact]
    public void EnsureCloudCompatibilityOrThrow_ValidCloud_ReturnsCloudConfig()
    {
        var expectedCloud = new CloudConfig { CdnUrl = "https://cdn.ckeditor.com" };
        var preset = new PresetConfig
        {
            Cloud = expectedCloud,
            LicenseKey = new LicenseKey("raw", DistributionChannel.Cloud)
        };

        var result = preset.EnsureCloudCompatibilityOrThrow();

        Assert.Same(expectedCloud, result);
    }

    [Fact]
    public void EnsureCloudCompatibilityOrThrow_CustomCdnWithCompatibleLicense_ReturnsCloudConfig()
    {
        var expectedCloud = new CloudConfig { CdnUrl = "https://custom.cdn.com" };
        var preset = new PresetConfig
        {
            Cloud = expectedCloud,
            LicenseKey = new LicenseKey("raw", null)
        };

        var result = preset.EnsureCloudCompatibilityOrThrow();

        Assert.Same(expectedCloud, result);
    }

    [Fact]
    public void EnsureSelfHostedCompatibilityOrThrow_IncompatibleLicense_ThrowsSelfHostedLicenseIncompatibleException()
    {
        var preset = new PresetConfig
        {
            LicenseKey = new LicenseKey("raw", DistributionChannel.Cloud)
        };

        Assert.Throws<SelfHostedLicenseIncompatibleException>(() => preset.EnsureSelfHostedCompatibilityOrThrow());
    }

    [Fact]
    public void EnsureSelfHostedCompatibilityOrThrow_ValidLicense_ReturnsSelfHostedConfig()
    {
        var preset = new PresetConfig
        {
            LicenseKey = new LicenseKey("raw", DistributionChannel.SH)
        };

        var result = preset.EnsureSelfHostedCompatibilityOrThrow();

        Assert.Same(preset.SelfHosted, result);
    }
}
