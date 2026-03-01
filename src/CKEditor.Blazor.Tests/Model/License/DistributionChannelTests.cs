using CKEditor.Blazor.Model.License;

namespace CKEditor.Blazor.Tests.Model.License;

public class DistributionChannelTests
{
    [Fact]
    public void DistributionChannel_ShouldHaveExpectedValues()
    {
        Assert.Equal(0, (int)DistributionChannel.SH);
        Assert.Equal(1, (int)DistributionChannel.Cloud);
    }
}
