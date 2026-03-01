using CKEditor.Blazor.Model.License;
using CKEditor.Blazor.Serialization;
using CKEditor.Blazor.Tests.Helpers;

namespace CKEditor.Blazor.Tests.Serialization;

public class LicenseKeyParserTests
{
    [Fact]
    public void Parse_GPL_ShouldReturnGPLKey()
    {
        var result = LicenseKeyParser.Parse("GPL");

        Assert.True(result.IsGPL());
        Assert.Equal("GPL", result.Raw);
    }

    [Fact]
    public void Parse_JWT_WithCloudChannel_ShouldReturnCloudDistributionChannel()
    {
        var jwt = JwtTestHelper.Build(distributionChannel: "cloud");

        var result = LicenseKeyParser.Parse(jwt);

        Assert.Equal(DistributionChannel.Cloud, result.DistributionChannel);
        Assert.Equal(jwt, result.Raw);
    }

    [Fact]
    public void Parse_JWT_WithSHChannel_ShouldReturnSHDistributionChannel()
    {
        var jwt = JwtTestHelper.Build(distributionChannel: "sh");

        var result = LicenseKeyParser.Parse(jwt);

        Assert.Equal(DistributionChannel.SH, result.DistributionChannel);
    }

    [Fact]
    public void Parse_JWT_WithoutChannel_ShouldReturnNullDistributionChannel()
    {
        var jwt = JwtTestHelper.Build();

        var result = LicenseKeyParser.Parse(jwt);

        Assert.Null(result.DistributionChannel);
    }

    [Fact]
    public void Parse_JWT_WithNullChannel_ShouldHaveNullDistributionChannel()
    {
        var jwt = JwtTestHelper.Build(extraClaims: new Dictionary<string, object?> { ["distributionChannel"] = null });

        var result = LicenseKeyParser.Parse(jwt);

        Assert.Null(result.DistributionChannel);
    }

    [Fact]
    public void Parse_JWT_WithExpiry_ShouldSetExpiresAt()
    {
        var expiry = DateTimeOffset.UtcNow.AddDays(30).ToUnixTimeSeconds();
        var jwt = JwtTestHelper.Build(exp: expiry);

        var result = LicenseKeyParser.Parse(jwt);

        Assert.Equal(expiry, result.ExpiresAt);
    }

    [Fact]
    public void Parse_JWT_ExpiredToken_ShouldBeMarkedAsExpired()
    {
        var jwt = JwtTestHelper.BuildExpired();

        var result = LicenseKeyParser.Parse(jwt);

        Assert.True(result.IsExpired());
    }

    [Fact]
    public void Parse_JWT_ValidToken_ShouldNotBeExpired()
    {
        var jwt = JwtTestHelper.BuildValid();

        var result = LicenseKeyParser.Parse(jwt);

        Assert.False(result.IsExpired());
    }

    [Fact]
    public void Parse_JWT_UpperCaseChannel_ShouldParseCorrectly()
    {
        // distributionChannel matching is case-insensitive (toLowerInvariant in parser)
        var jwt = JwtTestHelper.Build(distributionChannel: "Cloud");

        var result = LicenseKeyParser.Parse(jwt);

        Assert.Equal(DistributionChannel.Cloud, result.DistributionChannel);
    }

    [Fact]
    public void Parse_JWT_WithCloudChannel_ShouldBeCompatibleWithCloud()
    {
        var jwt = JwtTestHelper.BuildValid("cloud");

        var result = LicenseKeyParser.Parse(jwt);

        Assert.True(result.IsCompatibleWithCloud());
        Assert.False(result.IsCompatibleWithSelfHosted());
    }

    [Fact]
    public void Parse_JWT_WithSHChannel_ShouldBeCompatibleWithSelfHosted()
    {
        var jwt = JwtTestHelper.BuildValid("sh");

        var result = LicenseKeyParser.Parse(jwt);

        Assert.True(result.IsCompatibleWithSelfHosted());
        Assert.False(result.IsCompatibleWithCloud());
    }

    [Fact]
    public void Parse_JWT_WithUnknownChannel_ShouldThrow()
    {
        var jwt = JwtTestHelper.Build(distributionChannel: "unknown-channel");

        Assert.Throws<ArgumentException>(() => LicenseKeyParser.Parse(jwt));
    }

    [Fact]
    public void Parse_EmptyString_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => LicenseKeyParser.Parse(string.Empty));
    }

    [Fact]
    public void Parse_MissingPayloadPart_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => LicenseKeyParser.Parse("onlyonepart"));
    }

    [Fact]
    public void Parse_InvalidBase64Payload_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => LicenseKeyParser.Parse("header.!!!invalid_base64!!!.sig"));
    }

    [Fact]
    public void Parse_NullPayload_ShouldThrow()
    {
        // "bnVsbA" is the base64url encoding of "null"
        var ex = Assert.Throws<ArgumentException>(() => LicenseKeyParser.Parse("header.bnVsbA.sig"));
        Assert.Equal("Invalid JSON in JWT payload", ex.Message);
    }

    [Fact]
    public void TryParse_ValidGPL_ShouldReturnTrue()
    {
        var success = LicenseKeyParser.TryParse("GPL", out var key);

        Assert.True(success);
        Assert.NotNull(key);
        Assert.True(key!.IsGPL());
    }

    [Fact]
    public void TryParse_ValidJWT_ShouldReturnTrue()
    {
        var jwt = JwtTestHelper.Build(distributionChannel: "cloud");

        var success = LicenseKeyParser.TryParse(jwt, out var key);

        Assert.True(success);
        Assert.NotNull(key);
        Assert.Equal(DistributionChannel.Cloud, key!.DistributionChannel);
    }

    [Fact]
    public void TryParse_NullKey_ShouldReturnFalse()
    {
        var success = LicenseKeyParser.TryParse(null, out var key);

        Assert.False(success);
        Assert.Null(key);
    }

    [Fact]
    public void TryParse_EmptyKey_ShouldReturnFalse()
    {
        var success = LicenseKeyParser.TryParse(string.Empty, out var key);

        Assert.False(success);
        Assert.Null(key);
    }

    [Fact]
    public void TryParse_InvalidJWT_ShouldReturnFalse()
    {
        var success = LicenseKeyParser.TryParse("not.a.valid.jwt.at.all", out var key);

        Assert.False(success);
        Assert.Null(key);
    }
}
