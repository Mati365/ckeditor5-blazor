using CKEditor.Blazor.Model.License;

namespace CKEditor.Blazor.Tests.Model.License;

public class LicenseKeyTests
{
    [Fact]
    public void LicenseKey_ShouldInitializeCorrectly()
    {
        var raw = "test-token";
        var channel = DistributionChannel.Cloud;
        long expiresAt = 1234567890;

        var key = new LicenseKey(raw, channel, expiresAt);

        Assert.Equal(raw, key.Raw);
        Assert.Equal(channel, key.DistributionChannel);
        Assert.Equal(expiresAt, key.ExpiresAt);
    }

    [Fact]
    public void OfGPL_ShouldReturnGPLLicense()
    {
        var key = LicenseKey.OfGPL();

        Assert.Equal("GPL", key.Raw);
        Assert.Equal(DistributionChannel.SH, key.DistributionChannel);
        Assert.Null(key.ExpiresAt);
        Assert.True(key.IsGPL());
    }

    [Fact]
    public void IsExpired_ShouldReturnExpectedResult()
    {
        var expiredTime = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds();
        var validTime = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds();

        var expiredKey = new LicenseKey("raw", null, expiredTime);
        var validKey = new LicenseKey("raw", null, validTime);
        var noExpireKey = new LicenseKey("raw");

        Assert.True(expiredKey.IsExpired());
        Assert.False(validKey.IsExpired());
        Assert.False(noExpireKey.IsExpired());
    }

    [Fact]
    public void IsGPL_ShouldReturnExpectedResult()
    {
        var gplKey = new LicenseKey("GPL");
        var nonGplKey = new LicenseKey("other");

        Assert.True(gplKey.IsGPL());
        Assert.False(nonGplKey.IsGPL());
    }

    [Fact]
    public void IsCloudOnly_ShouldReturnExpectedResult()
    {
        var cloudKey = new LicenseKey("raw", DistributionChannel.Cloud);
        var shKey = new LicenseKey("raw", DistributionChannel.SH);
        var noneKey = new LicenseKey("raw");

        Assert.True(cloudKey.IsCloudOnly());
        Assert.False(shKey.IsCloudOnly());
        Assert.False(noneKey.IsCloudOnly());
    }

    [Fact]
    public void IsSelfHostedOnly_ShouldReturnExpectedResult()
    {
        var shKey = new LicenseKey("raw", DistributionChannel.SH);
        var cloudKey = new LicenseKey("raw", DistributionChannel.Cloud);
        var noneKey = new LicenseKey("raw");

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
        var key = new LicenseKey("raw", channel);

        Assert.Equal(expected, key.IsCompatibleWithCloud());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(DistributionChannel.SH, true)]
    [InlineData(DistributionChannel.Cloud, false)]
    public void IsCompatibleWithSelfHosted_ShouldReturnExpectedResult(DistributionChannel? channel, bool expected)
    {
        var key = new LicenseKey("raw", channel);

        Assert.Equal(expected, key.IsCompatibleWithSelfHosted());
    }
}
