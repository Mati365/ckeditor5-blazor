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
        // Arrange
        var preset = new PresetConfig { Cloud = null };

        // Act & Assert
        Assert.Throws<CloudConfigurationMissingException>(() => preset.EnsureCloudCompatibilityOrThrow());
    }

    [Fact]
    public void EnsureCloudCompatibilityOrThrow_CustomCdnWithCloudOnlyLicense_ThrowsCloudLicenseIncompatibleException()
    {
        // Arrange
        var preset = new PresetConfig
        {
            Cloud = new CloudConfig { CdnUrl = "https://custom.cdn.com" },
            LicenseKey = new LicenseKey("raw", DistributionChannel.Cloud)
        };

        // Act & Assert
        Assert.Throws<CloudLicenseIncompatibleException>(() => preset.EnsureCloudCompatibilityOrThrow());
    }

    [Fact]
    public void EnsureCloudCompatibilityOrThrow_OfficialCdnWithIncompatibleLicense_ThrowsCloudLicenseIncompatibleException()
    {
        // Arrange
        var preset = new PresetConfig
        {
            Cloud = new CloudConfig { CdnUrl = "https://cdn.ckeditor.com" },
            LicenseKey = new LicenseKey("raw", DistributionChannel.SH)
        };

        // Act & Assert
        Assert.Throws<CloudLicenseIncompatibleException>(() => preset.EnsureCloudCompatibilityOrThrow());
    }

    [Fact]
    public void EnsureCloudCompatibilityOrThrow_ValidCloud_ReturnsCloudConfig()
    {
        // Arrange
        var expectedCloud = new CloudConfig { CdnUrl = "https://cdn.ckeditor.com" };
        var preset = new PresetConfig
        {
            Cloud = expectedCloud,
            LicenseKey = new LicenseKey("raw", DistributionChannel.Cloud)
        };

        // Act
        var result = preset.EnsureCloudCompatibilityOrThrow();

        // Assert
        Assert.Same(expectedCloud, result);
    }

    [Fact]
    public void EnsureCloudCompatibilityOrThrow_CustomCdnWithCompatibleLicense_ReturnsCloudConfig()
    {
        // Arrange
        var expectedCloud = new CloudConfig { CdnUrl = "https://custom.cdn.com" };
        var preset = new PresetConfig
        {
            Cloud = expectedCloud,
            LicenseKey = new LicenseKey("raw", null)
        };

        // Act
        var result = preset.EnsureCloudCompatibilityOrThrow();

        // Assert
        Assert.Same(expectedCloud, result);
    }

    [Fact]
    public void EnsureSelfHostedCompatibilityOrThrow_IncompatibleLicense_ThrowsSelfHostedLicenseIncompatibleException()
    {
        // Arrange
        var preset = new PresetConfig
        {
            LicenseKey = new LicenseKey("raw", DistributionChannel.Cloud)
        };

        // Act & Assert
        Assert.Throws<SelfHostedLicenseIncompatibleException>(() => preset.EnsureSelfHostedCompatibilityOrThrow());
    }

    [Fact]
    public void EnsureSelfHostedCompatibilityOrThrow_ValidLicense_ReturnsSelfHostedConfig()
    {
        // Arrange
        var preset = new PresetConfig
        {
            LicenseKey = new LicenseKey("raw", DistributionChannel.SH)
        };

        // Act
        var result = preset.EnsureSelfHostedCompatibilityOrThrow();

        // Assert
        Assert.Same(preset.SelfHosted, result);
    }
}
