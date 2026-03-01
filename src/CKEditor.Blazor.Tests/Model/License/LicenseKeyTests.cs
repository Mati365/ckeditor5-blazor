using CKEditor.Blazor.Model.License;

namespace CKEditor.Blazor.Tests.Model.License;

public class LicenseKeyTests
{
    [Fact]
    public void LicenseKey_ShouldInitializeCorrectly()
    {
        // Arrange
        var raw = "test-token";
        var channel = DistributionChannel.Cloud;
        long expiresAt = 1234567890;

        // Act
        var key = new LicenseKey(raw, channel, expiresAt);

        // Assert
        Assert.Equal(raw, key.Raw);
        Assert.Equal(channel, key.DistributionChannel);
        Assert.Equal(expiresAt, key.ExpiresAt);
    }

    [Fact]
    public void OfGPL_ShouldReturnGPLLicense()
    {
        // Act
        var key = LicenseKey.OfGPL();

        // Assert
        Assert.Equal("GPL", key.Raw);
        Assert.Equal(DistributionChannel.SH, key.DistributionChannel);
        Assert.Null(key.ExpiresAt);
        Assert.True(key.IsGPL());
    }

    [Fact]
    public void IsExpired_ShouldReturnExpectedResult()
    {
        // Arrange
        var expiredTime = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds();
        var validTime = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds();

        var expiredKey = new LicenseKey("raw", null, expiredTime);
        var validKey = new LicenseKey("raw", null, validTime);
        var noExpireKey = new LicenseKey("raw");

        // Act & Assert
        Assert.True(expiredKey.IsExpired());
        Assert.False(validKey.IsExpired());
        Assert.False(noExpireKey.IsExpired());
    }

    [Fact]
    public void IsGPL_ShouldReturnExpectedResult()
    {
        // Arrange
        var gplKey = new LicenseKey("GPL");
        var nonGplKey = new LicenseKey("other");

        // Act & Assert
        Assert.True(gplKey.IsGPL());
        Assert.False(nonGplKey.IsGPL());
    }

    [Fact]
    public void IsCloudOnly_ShouldReturnExpectedResult()
    {
        // Arrange
        var cloudKey = new LicenseKey("raw", DistributionChannel.Cloud);
        var shKey = new LicenseKey("raw", DistributionChannel.SH);
        var noneKey = new LicenseKey("raw");

        // Act & Assert
        Assert.True(cloudKey.IsCloudOnly());
        Assert.False(shKey.IsCloudOnly());
        Assert.False(noneKey.IsCloudOnly());
    }

    [Fact]
    public void IsSelfHostedOnly_ShouldReturnExpectedResult()
    {
        // Arrange
        var shKey = new LicenseKey("raw", DistributionChannel.SH);
        var cloudKey = new LicenseKey("raw", DistributionChannel.Cloud);
        var noneKey = new LicenseKey("raw");

        // Act & Assert
        Assert.True(shKey.IsSelfHostedOnly());
        Assert.False(cloudKey.IsSelfHostedOnly());
        Assert.False(noneKey.IsSelfHostedOnly());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(DistributionChannel.Cloud, true)]
    [InlineData(DistributionChannel.SH, false)]
    public void IsCompatibleWithCloud_ShouldReturnExpectedResult(DistributionChannel? channel, bool expected)
    {
        // Arrange
        var key = new LicenseKey("raw", channel);

        // Act & Assert
        Assert.Equal(expected, key.IsCompatibleWithCloud());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(DistributionChannel.SH, true)]
    [InlineData(DistributionChannel.Cloud, false)]
    public void IsCompatibleWithSelfHosted_ShouldReturnExpectedResult(DistributionChannel? channel, bool expected)
    {
        // Arrange
        var key = new LicenseKey("raw", channel);

        // Act & Assert
        Assert.Equal(expected, key.IsCompatibleWithSelfHosted());
    }
}
