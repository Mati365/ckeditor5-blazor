using CKEditor.Blazor.Exceptions;
using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.SelfHosted;
using CKEditor.Blazor.Services.Bundle.SelfHosted;
using CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;
using Moq;

namespace CKEditor.Blazor.Tests.Services.Bundle.SelfHosted;

public class SelfHostedBundleBuilderTests
{
    private readonly Mock<ICKEditorSelfHostedBundleBuilder> _editorBuilderMock = new();
    private readonly Mock<ICKEditorPremiumSelfHostedBundleBuilder> _premiumBuilderMock = new();
    private readonly Mock<ICKBoxSelfHostedBundleBuilder> _ckboxBuilderMock = new();
    private readonly SelfHostedBundleBuilder _sut;

    public SelfHostedBundleBuilderTests()
    {
        _sut = new SelfHostedBundleBuilder(
            _editorBuilderMock.Object,
            _premiumBuilderMock.Object,
            _ckboxBuilderMock.Object
        );
    }

    [Fact]
    public void Build_ThrowsConfigurationException_WhenEditorVersionIsNull()
    {
        var config = new SelfHostedConfig { EditorVersion = null! };

        var ex = Assert.Throws<ConfigurationException>(() => _sut.Build(config));
        Assert.Equal("Self-hosted config requires 'EditorVersion'.", ex.Message);
    }

    [Fact]
    public void Build_ThrowsConfigurationException_WhenCKBoxVersionIsNullAndCKBoxIsEnabled()
    {
        var config = new SelfHostedConfig
        {
            EditorVersion = "40.0.0",
            CKBox = new CKBoxSelfHostedConfig { Version = null! }
        };

        var ex = Assert.Throws<ConfigurationException>(() => _sut.Build(config));
        Assert.Equal("Self-hosted config requires CKBox 'Version' when CKBox is enabled.", ex.Message);
    }

    [Fact]
    public void Build_AggregatesBundlesAndAppendsBlazorIntegrationAsset()
    {
        // Arrange
        var editorBundle = new AssetsBundle([new JSAsset { Name = "ckeditor5", Url = "edit.js" }], ["edit.css"]);
        var premiumBundle = new AssetsBundle([new JSAsset { Name = "premium", Url = "prem.js" }], ["prem.css"]);
        var ckboxBundle = new AssetsBundle([new JSAsset { Name = "ckbox", Url = "ckb.js" }], ["ckb.css"]);

        _editorBuilderMock.Setup(b => b.Build("40.0.0", "custom-path")).Returns(editorBundle);
        _premiumBuilderMock.Setup(b => b.Build("40.0.0", "custom-path")).Returns(premiumBundle);
        _ckboxBuilderMock.Setup(b => b.Build("2.1.0", It.IsAny<IReadOnlyList<string>>(), "custom-path", "lark")).Returns(ckboxBundle);

        var config = new SelfHostedConfig
        {
            EditorVersion = "40.0.0",
            AssetsBasePath = "custom-path",
            Premium = true,
            CKBox = new CKBoxSelfHostedConfig
            {
                Version = "2.1.0",
                Translations = ["en"]
            }
        };

        // Act
        var result = _sut.Build(config);

        // Assert
        Assert.Contains(result.Js, js => js.Name == "ckeditor5" && js.Url == "edit.js");
        Assert.Contains(result.Js, js => js.Name == "premium" && js.Url == "prem.js");
        Assert.Contains(result.Js, js => js.Name == "ckbox" && js.Url == "ckb.js");
        Assert.Contains(result.Js, js => js.Name == AssetsBundle.GetBlazorIntegrationAsset(config.IntegrationBasePath).Name && js.Url == AssetsBundle.GetBlazorIntegrationAsset(config.IntegrationBasePath).Url);

        Assert.Contains(result.Css, css => css == "edit.css");
        Assert.Contains(result.Css, css => css == "prem.css");
        Assert.Contains(result.Css, css => css == "ckb.css");

        Assert.Equal(4, result.Js.Count);
        Assert.Equal(3, result.Css.Count);

        _editorBuilderMock.Verify(b => b.Build("40.0.0", "custom-path"), Times.Once);
        _premiumBuilderMock.Verify(b => b.Build("40.0.0", "custom-path"), Times.Once);
        _ckboxBuilderMock.Verify(b => b.Build("2.1.0", config.CKBox.Translations, "custom-path", "lark"), Times.Once);
    }

    [Fact]
    public void Build_OnlyIncludesCoreEditorAndBlazorIntegration_WhenOthersAreDisabled()
    {
        // Arrange
        var editorBundle = new AssetsBundle([new JSAsset { Name = "ckeditor5" }], ["edit.css"]);
        _editorBuilderMock.Setup(b => b.Build("39.0.0", "default-path")).Returns(editorBundle);

        var config = new SelfHostedConfig
        {
            EditorVersion = "39.0.0",
            AssetsBasePath = "default-path",
            Premium = false,
            CKBox = null
        };

        // Act
        var result = _sut.Build(config);

        // Assert
        Assert.Equal(2, result.Js.Count); // ckeditor5 + blazor-integration
        Assert.Contains(result.Js, js => js.Name == "ckeditor5");
        Assert.Contains(result.Js, js => js.Name == AssetsBundle.GetBlazorIntegrationAsset(config.IntegrationBasePath).Name);

        Assert.Single(result.Css);
        Assert.Contains(result.Css, css => css == "edit.css");

        _premiumBuilderMock.Verify(b => b.Build(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _ckboxBuilderMock.Verify(b => b.Build(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
