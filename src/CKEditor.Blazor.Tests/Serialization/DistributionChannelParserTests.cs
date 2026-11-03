using System.Text.Json;
using CKEditor.Blazor.Model.License;
using CKEditor.Blazor.Serialization;

namespace CKEditor.Blazor.Tests.Serialization;

public class DistributionChannelParserTests
{
    [Fact]
    public void Parse_Null_ShouldReturnNull()
    {
        Assert.Null(Parse("null"));
    }

    [Theory]
    [InlineData("\"cloud\"", DistributionChannel.Cloud)]
    [InlineData("\"Cloud\"", DistributionChannel.Cloud)]
    [InlineData("\"sh\"", DistributionChannel.SH)]
    [InlineData("\"SH\"", DistributionChannel.SH)]
    public void Parse_String_ShouldReturnMatchingChannel(string json, DistributionChannel expected)
    {
        Assert.Equal(expected, Parse(json));
    }

    [Fact]
    public void Parse_UnknownString_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(static () => Parse("\"unknown-channel\""));
    }

    [Theory]
    [InlineData("[\"cloud\"]", DistributionChannel.Cloud)]
    [InlineData("[\"sh\"]", DistributionChannel.SH)]
    [InlineData("[\"drupal\", \"SH\", \"something-else\"]", DistributionChannel.SH)]
    [InlineData("[\"sh\", \"cloud\"]", DistributionChannel.Cloud)]
    [InlineData("[42, \"cloud\", null]", DistributionChannel.Cloud)]
    public void Parse_Array_ShouldReturnKnownChannel(string json, DistributionChannel expected)
    {
        Assert.Equal(expected, Parse(json));
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("[\"drupal\", \"unknown\"]")]
    public void Parse_ArrayWithoutKnownChannel_ShouldReturnNull(string json)
    {
        Assert.Null(Parse(json));
    }

    [Theory]
    [InlineData("42")]
    [InlineData("true")]
    [InlineData("{\"channel\": \"cloud\"}")]
    public void Parse_UnsupportedType_ShouldThrow(string json)
    {
        Assert.Throws<ArgumentException>(() => Parse(json));
    }

    private static DistributionChannel? Parse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return DistributionChannelParser.Parse(doc.RootElement);
    }
}
