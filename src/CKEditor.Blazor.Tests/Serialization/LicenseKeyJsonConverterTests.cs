using System.Text.Json;
using CKEditor.Blazor.Model.License;
using CKEditor.Blazor.Serialization;
using CKEditor.Blazor.Tests.Helpers;

namespace CKEditor.Blazor.Tests.Serialization;

public class LicenseKeyJsonConverterTests
{
    private static readonly JsonSerializerOptions _options = new()
    {
        Converters = { new LicenseKeyJsonConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Fact]
    public void Read_GPLString_ShouldReturnGPLKey()
    {
        var json = """{"key":"GPL"}""";

        var wrapper = JsonSerializer.Deserialize<KeyWrapper>(json, _options);

        Assert.NotNull(wrapper?.Key);
        Assert.True(wrapper!.Key!.IsGPL());
    }

    [Fact]
    public void Read_ValidJWT_ShouldReturnParsedKey()
    {
        var jwt = JwtTestHelper.Build(distributionChannel: "cloud");
        var json = $$"""{ "key": "{{jwt}}" }""";

        var wrapper = JsonSerializer.Deserialize<KeyWrapper>(json, _options);

        Assert.NotNull(wrapper?.Key);
        Assert.Equal(DistributionChannel.Cloud, wrapper!.Key!.DistributionChannel);
    }

    [Fact]
    public void Read_NullString_ShouldReturnNull()
    {
        var json = """{"key":null}""";

        var wrapper = JsonSerializer.Deserialize<KeyWrapperNullable>(json, _options);

        Assert.Null(wrapper?.Key);
    }

    [Fact]
    public void Read_NullTokenDirectly_ShouldReturnNull()
    {
        var json = "null";
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
        reader.Read();

        var converter = new LicenseKeyJsonConverter();
        var result = converter.Read(ref reader, typeof(LicenseKey), _options);

        Assert.Null(result);
    }

    [Fact]
    public void Write_GPLKey_ShouldSerializeRawString()
    {
        var wrapper = new KeyWrapper { Key = LicenseKey.OfGPL() };

        var json = JsonSerializer.Serialize(wrapper, _options);

        Assert.Contains("\"GPL\"", json);
    }

    [Fact]
    public void Write_JWTKey_ShouldSerializeRawJWTString()
    {
        var jwt = JwtTestHelper.Build(distributionChannel: "sh");
        var key = new LicenseKey(jwt, DistributionChannel.SH, null);
        var wrapper = new KeyWrapper { Key = key };

        var json = JsonSerializer.Serialize(wrapper, _options);

        Assert.Contains(jwt, json);
    }

    [Fact]
    public void RoundTrip_JWTKey_ShouldPreserveRawToken()
    {
        var jwt = JwtTestHelper.BuildValid("cloud");
        var original = new KeyWrapper { Key = new LicenseKey(jwt, DistributionChannel.Cloud, null) };

        var json = JsonSerializer.Serialize(original, _options);
        var restored = JsonSerializer.Deserialize<KeyWrapper>(json, _options);

        Assert.Equal(jwt, restored?.Key?.Raw);
        Assert.Equal(DistributionChannel.Cloud, restored?.Key?.DistributionChannel);
    }

    private class KeyWrapper
    {
        public LicenseKey? Key { get; set; }
    }

    private class KeyWrapperNullable
    {
        public LicenseKey? Key { get; set; }
    }
}
