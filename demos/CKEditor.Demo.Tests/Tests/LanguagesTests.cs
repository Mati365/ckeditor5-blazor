using CKEditor.Demo.Tests.Infrastructure;

namespace CKEditor.Demo.Tests.Tests;

public class LanguagesTests : PageTestBase, IClassFixture<BrowserFixture>
{
    protected override string PagePath => "/languages";

    public LanguagesTests(BrowserFixture fixture) : base(fixture) { }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        await WaitForEditorAsync(index: 0);
        await WaitForEditorAsync(index: 1);
        await WaitForEditorAsync(index: 2);
    }

    [Fact]
    public async Task Page_RendersThreeInteractiveEditors()
    {
        var editors = Page.Locator("cke5-editor[data-cke-interactive='true']");
        await Assertions.Expect(editors).ToHaveCountAsync(3);
    }

    [Fact]
    public async Task FirstEditor_RendersWithPolishInitialContent()
    {
        var firstEditorEditable = Page
            .Locator("cke5-editor")
            .Nth(0)
            .Locator(".ck-editor__editable");

        await Assertions.Expect(firstEditorEditable)
            .ToContainTextAsync("Ten edytor jest skonfigurowany w języku polskim.");
    }

    [Fact]
    public async Task SecondEditor_BoldButton_HasCustomLabel_Boldify()
    {
        var secondEditorToolbar = Page
            .Locator("cke5-editor")
            .Nth(1)
            .Locator(".ck-toolbar");

        var boldButton = secondEditorToolbar.Locator("[data-cke-tooltip-text='BOLDIFY! (Ctrl+B)']");
        await Assertions.Expect(boldButton).ToBeVisibleAsync();
    }

    [Fact]
    public async Task SecondEditor_ItalicButton_HasCustomLabel_Slant()
    {
        var secondEditorToolbar = Page
            .Locator("cke5-editor")
            .Nth(1)
            .Locator(".ck-toolbar");

        var italicButton = secondEditorToolbar.Locator("[data-cke-tooltip-text='SLANT! (Ctrl+I)']");
        await Assertions.Expect(italicButton).ToBeVisibleAsync();
    }

    [Fact]
    public async Task ThirdEditor_EditableContent_HasRtlDirection()
    {
        var thirdEditorContent = Page
            .Locator("cke5-editor")
            .Nth(2)
            .Locator(".ck-content");

        await Assertions.Expect(thirdEditorContent)
            .ToHaveAttributeAsync("dir", "rtl");
    }

    [Fact]
    public async Task ThirdEditor_RendersArabicInitialContent()
    {
        var thirdEditorEditable = Page
            .Locator("cke5-editor")
            .Nth(2)
            .Locator(".ck-editor__editable");

        await Assertions.Expect(thirdEditorEditable).Not.ToBeEmptyAsync();
    }
}
