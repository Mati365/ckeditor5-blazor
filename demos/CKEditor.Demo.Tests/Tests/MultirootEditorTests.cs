using CKEditor.Demo.Tests.Infrastructure;

namespace CKEditor.Demo.Tests.Tests;

public class MultirootEditorTests : PageTestBase, IClassFixture<BrowserFixture>
{
    protected override string PagePath => "/multiroot";

    public MultirootEditorTests(BrowserFixture fixture) : base(fixture) { }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        await Page.Locator("cke5-editor[data-cke-interactive='true']").First
            .WaitForAsync(new() { State = WaitForSelectorState.Attached, Timeout = 30_000 });

        await WaitForEditableAsync(index: 0);
        await WaitForEditableAsync(index: 1);
        await WaitForEditableAsync(index: 2);

        await Assertions.Expect(Page.Locator(".bg-green-50"))
            .ToContainTextAsync("footer:", new() { Timeout = 15_000 });
    }

    [Fact]
    public async Task Editor_RendersThreeInitialRoots()
    {
        await Assertions.Expect(Page.Locator("#container-header")).ToBeVisibleAsync();
        await Assertions.Expect(Page.Locator("#container-content")).ToBeVisibleAsync();
        await Assertions.Expect(Page.Locator("#container-footer")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Editor_InitialRootValues_AppearsInServerDisplay()
    {
        await Assertions.Expect(Page.Locator("#container-header p.text-gray-400"))
            .ToContainTextAsync("Header content");
        await Assertions.Expect(Page.Locator("#container-content p.text-gray-400"))
            .ToContainTextAsync("Main content");
        await Assertions.Expect(Page.Locator("#container-footer p.text-gray-400"))
            .ToContainTextAsync("Footer content");
    }

    [Fact]
    public async Task AddRoot_CreatesNewEditableContainer()
    {
        await Page.Locator("input[placeholder*='root ID']").FillAsync("newsidebar");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add Root" }).ClickAsync();

        await Assertions.Expect(Page.Locator("#container-newsidebar")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task AddRoot_WithBlankName_GeneratesRandomRootId()
    {
        var input = Page.Locator("input[placeholder*='root ID']");

        await input.ClearAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add Root" }).ClickAsync();

        await Assertions.Expect(
            Page.Locator("[id^='container-r_']")
        ).ToBeVisibleAsync();
    }

    [Fact]
    public async Task RemoveRoot_DeletesContainerFromDom()
    {
        await Page.Locator("#container-header")
            .GetByRole(AriaRole.Button, new() { Name = "Remove" })
            .ClickAsync();

        await Assertions.Expect(Page.Locator("#container-header")).ToBeHiddenAsync();
    }

    [Fact]
    public async Task EditableContent_SyncsValueToServerDisplay()
    {
        var headerEditable = Page
            .Locator("#container-header .ck-editor__editable[contenteditable='true']");

        await headerEditable.ClickAsync();
        await Page.Keyboard.PressAsync("Control+A");
        await headerEditable.PressSequentiallyAsync("Updated header text");

        await Assertions.Expect(Page.Locator("#container-header p.text-gray-400"))
            .ToContainTextAsync("Updated header text", new() { Timeout = 10_000 });
        await Assertions.Expect(Page.Locator("#container-header p.text-gray-400"))
            .Not.ToContainTextAsync("Header content");
    }

    [Fact]
    public async Task EditableChange_FiresOnRootChangeEvent()
    {
        var contentEditable = Page
            .Locator("#container-content .ck-editor__editable[contenteditable='true']");

        await contentEditable.ClickAsync();
        await contentEditable.PressSequentiallyAsync("Root change event test");

        await Assertions.Expect(Page.Locator(".bg-green-50"))
            .ToContainTextAsync("content: ", new() { Timeout = 10_000 });
        await Assertions.Expect(Page.Locator(".bg-green-50"))
            .ToContainTextAsync("Root change event test");
    }

    [Fact]
    public async Task EditorChange_FiresOnEditorChangeEvent()
    {
        var footerEditable = Page
            .Locator("#container-footer .ck-editor__editable[contenteditable='true']");

        await footerEditable.ClickAsync();
        await footerEditable.PressSequentiallyAsync("Editor level change");

        await Assertions.Expect(Page.GetByText("Last editor OnChange payload:"))
            .ToBeVisibleAsync(new() { Timeout = 10_000 });
        await Assertions.Expect(Page.Locator(".bg-yellow-50 pre"))
            .ToContainTextAsync("Editor level change");
    }

    [Fact]
    public async Task ClearValues_RemovesContentFromAllRoots()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Clear Values" }).ClickAsync();

        await Assertions.Expect(Page.Locator("#container-header p.text-gray-400"))
            .Not.ToContainTextAsync("Header content", new() { Timeout = 5_000 });
        await Assertions.Expect(Page.Locator("#container-content p.text-gray-400"))
            .Not.ToContainTextAsync("Main content", new() { Timeout = 5_000 });
        await Assertions.Expect(Page.Locator("#container-footer p.text-gray-400"))
            .Not.ToContainTextAsync("Footer content", new() { Timeout = 5_000 });
    }
}
