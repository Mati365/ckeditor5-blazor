using CKEditor.Demo.Tests.Infrastructure;

namespace CKEditor.Demo.Tests.Tests;

public class FormEditorTests : PageTestBase, IClassFixture<BrowserFixture>
{
    protected override string PagePath => "/form";

    public FormEditorTests(BrowserFixture fixture) : base(fixture) { }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await WaitForEditorAsync();
    }

    [Fact]
    public async Task Editor_RendersInsideForm()
    {
        await Assertions.Expect(
            Page.Locator("cke5-editor[data-cke-interactive='true']")
        ).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Submit_WithInitialValue_ShowsSubmittedSection()
    {
        // Wait for editor to sync its initial value back.
        await Task.Delay(500);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();

        await Assertions.Expect(Page.GetByText("Submitted content:")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Submit_WithInitialValue_DisplaysEditorContent()
    {
        await Task.Delay(500);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();

        await Assertions.Expect(
            Page.Locator(".bg-green-100 div")
        ).ToContainTextAsync("Initial Value");
    }

    [Fact]
    public async Task Submit_AfterTyping_DisplaysTypedContent()
    {
        var editable = Page.Locator(".ck-editor__editable[contenteditable='true']");
        await editable.ClickAsync();
        await Page.Keyboard.PressAsync("Control+A");
        await editable.PressSequentiallyAsync("Form submission test");

        await Task.Delay(1500);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();

        await Assertions.Expect(
            Page.Locator(".bg-green-100 div")
        ).ToContainTextAsync("Form submission test");
    }
}
