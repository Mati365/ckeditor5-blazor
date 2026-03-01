using Moq;
using CKEditor.Blazor.Exceptions;
using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.Cloud;
using CKEditor.Blazor.Services.Bundle.Cloud;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;

namespace CKEditor.Blazor.Tests.Services.Bundle.Cloud;

public class CloudBundleBuilderTests
{
    private readonly Mock<ICKEditorCloudBundleBuilder> _editorBuilderMock = new();
    private readonly Mock<ICKEditorPremiumCloudBundleBuilder> _premiumBuilderMock = new();
    private readonly Mock<ICKBoxCloudBundleBuilder> _ckboxBuilderMock = new();
    private readonly CloudBundleBuilder _sut;

    public CloudBundleBuilderTests()
    {
        _sut = new CloudBundleBuilder(
            _editorBuilderMock.Object,
            _premiumBuilderMock.Object,
            _ckboxBuilderMock.Object
        );
    }

    [Fact]
    public void Build_ThrowsCloudConfigurationException_WhenEditorVersionIsNull()
    {
        var config = new CloudConfig { EditorVersion = null! };

        var ex = Assert.Throws<CloudConfigurationException>(() => _sut.Build(config));
        Assert.Equal("Cloud config requires 'EditorVersion'.", ex.Message);
    }

    [Fact]
    public void Build_ThrowsCloudConfigurationException_WhenCKBoxVersionIsNullAndCKBoxIsEnabled()
    {
        var config = new CloudConfig
        {
            EditorVersion = "40.0.0",
            CKBox = new CKBoxCloudConfig { Version = null! }
        };

        var ex = Assert.Throws<CloudConfigurationException>(() => _sut.Build(config));
        Assert.Equal("Cloud config requires CKBox 'Version' when CKBox is enabled.", ex.Message);
    }

    [Fact]
    public void Build_AggregatesBundlesAndAppendsBlazorIntegrationAsset()
    {
        // Arrange
        var editorBundle = new AssetsBundle([new JSAsset { Name = "ckeditor5", Url = "edit.js" }], ["edit.css"]);
        var premiumBundle = new AssetsBundle([new JSAsset { Name = "premium", Url = "prem.js" }], ["prem.css"]);
        var ckboxBundle = new AssetsBundle([new JSAsset { Name = "ckbox", Url = "ckb.js" }], ["ckb.css"]);

        _editorBuilderMock.Setup(b => b.Build("40.0.0", "custom-cdn")).Returns(editorBundle);
        _premiumBuilderMock.Setup(b => b.Build("40.0.0", "custom-cdn")).Returns(premiumBundle);
        _ckboxBuilderMock.Setup(b => b.Build("2.1.0", It.IsAny<IReadOnlyList<string>>(), "cdn-ckbox", "lark")).Returns(ckboxBundle);

        var config = new CloudConfig
        {
            EditorVersion = "40.0.0",
            CdnUrl = "custom-cdn",
            Premium = true,
            CKBox = new CKBoxCloudConfig
            {
                Version = "2.1.0",
                CdnUrl = "cdn-ckbox",
                Translations = [ "en" ]
            }
        };

        // Act
        var result = _sut.Build(config);

        // Assert
        Assert.Contains(result.Js, js => js.Name == "ckeditor5" && js.Url == "edit.js");
        Assert.Contains(result.Js, js => js.Name == "premium" && js.Url == "prem.js");
        Assert.Contains(result.Js, js => js.Name == "ckbox" && js.Url == "ckb.js");
        Assert.Contains(result.Js, js => js.Name == AssetsBundle.BlazorIntegrationAsset.Name && js.Url == AssetsBundle.BlazorIntegrationAsset.Url);

        Assert.Contains(result.Css, css => css == "edit.css");
        Assert.Contains(result.Css, css => css == "prem.css");
        Assert.Contains(result.Css, css => css == "ckb.css");

        Assert.Equal(4, result.Js.Count);
        Assert.Equal(3, result.Css.Count);

        _editorBuilderMock.Verify(b => b.Build("40.0.0", "custom-cdn"), Times.Once);
        _premiumBuilderMock.Verify(b => b.Build("40.0.0", "custom-cdn"), Times.Once);
        _ckboxBuilderMock.Verify(b => b.Build("2.1.0", config.CKBox.Translations, "cdn-ckbox", "lark"), Times.Once);
    }

    [Fact]
    public void Build_OnlyIncludesCoreEditorAndBlazorIntegration_WhenOthersAreDisabled()
    {
        // Arrange
        var editorBundle = new AssetsBundle([new JSAsset { Name = "ckeditor5" }], ["edit.css"]);
        _editorBuilderMock.Setup(b => b.Build("39.0.0", "default-cdn")).Returns(editorBundle);

        var config = new CloudConfig
        {
            EditorVersion = "39.0.0",
            CdnUrl = "default-cdn",
            Premium = false,
            CKBox = null
        };

        // Act
        var result = _sut.Build(config);

        // Assert
        Assert.Equal(2, result.Js.Count); // ckeditor5 + blazor-integration
        Assert.Contains(result.Js, js => js.Name == "ckeditor5");
        Assert.Contains(result.Js, js => js.Name == AssetsBundle.BlazorIntegrationAsset.Name);

        Assert.Single(result.Css);
        Assert.Contains(result.Css, css => css == "edit.css");

        _premiumBuilderMock.Verify(b => b.Build(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _ckboxBuilderMock.Verify(b => b.Build(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
