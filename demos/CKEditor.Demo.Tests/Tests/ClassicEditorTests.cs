using CKEditor.Demo.Tests.Infrastructure;

namespace CKEditor.Demo.Tests.Tests;

public partial class ClassicEditorTests : PageTestBase, IClassFixture<BrowserFixture>
{
    protected override string PagePath => "/classic";

    public ClassicEditorTests(BrowserFixture fixture) : base(fixture) { }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await WaitForEditorAsync();
    }

    [Fact]
    public async Task Editor_BecomesInteractive()
    {
        await Assertions.Expect(
            Page.Locator("cke5-editor[data-cke-interactive='true']")
        ).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Editor_OnReady_ShowsReadyMessage()
    {
        await Assertions.Expect(
            Page.GetByText("Editor instance is ready.")
        ).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Editor_InitialValue_AppearsInServerDisplay()
    {
        await Assertions.Expect(Page.Locator("pre").First)
            .ToContainTextAsync("Hello World");
    }

    [Fact]
    public async Task Editor_ContentTyped_SyncsToServer()
    {
        var editable = Page.Locator(".ck-editor__editable[contenteditable='true']");

        await editable.ClickAsync();
        await Page.Keyboard.PressAsync("Control+A");
        await editable.PressSequentiallyAsync("Blazor sync test");

        await Assertions.Expect(Page.Locator("pre").First)
            .ToContainTextAsync("Blazor sync test", new() { Timeout = 10_000 });
        await Assertions.Expect(Page.Locator("pre").First)
            .Not.ToContainTextAsync("Hello World");
    }

    [Fact]
    public async Task Editor_OnChange_EventPayloadIsDisplayed()
    {
        var editable = Page.Locator(".ck-editor__editable[contenteditable='true']");

        await editable.ClickAsync();
        await editable.PressSequentiallyAsync("OnChangePayloadTest");

        await Assertions.Expect(Page.GetByText("Last OnChange payload (JSON):"))
            .ToBeVisibleAsync(new() { Timeout = 10_000 });
        await Assertions.Expect(Page.Locator(".bg-yellow-50 pre"))
            .ToContainTextAsync("OnChangePayloadTest");
    }

    [Fact]
    public async Task Editor_SetData_PushesContentFromServer()
    {
        await Page.Locator("textarea").FillAsync("<p>Server-pushed content</p>");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Set Data" }).ClickAsync();

        await Assertions.Expect(Page.Locator("pre").First)
            .ToContainTextAsync("Server-pushed content", new() { Timeout = 10_000 });
        await Assertions.Expect(Page.Locator("pre").First)
            .Not.ToContainTextAsync("Hello World");
    }

    [Fact]
    public async Task Editor_LoadTemplate_LoadsTemplateContent()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Load Template" }).ClickAsync();

        await Assertions.Expect(Page.Locator("pre").First)
            .ToContainTextAsync("Work Report");
    }

    [Fact]
    public async Task Editor_Clear_RemovesOriginalContent()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Clear" }).ClickAsync();

        await Assertions.Expect(Page.Locator("pre").First)
            .Not.ToContainTextAsync("Hello World", new() { Timeout = 5_000 });
    }

    [Fact]
    public async Task Editor_FocusBlur_HighlightsBorderAccordingly()
    {
        var valueBox = Page.Locator("div").Filter(new() { HasText = "Current editor value (Server):" }).First;
        var editable = Page.Locator(".ck-editor__editable[contenteditable='true']");

        await editable.ClickAsync();

        await Assertions.Expect(valueBox).ToHaveCSSAsync("border-color", MyRegex());
    }

    [Fact]
    public void CustomPlugin_IsInitialized()
    {
        Assert.Contains("MyCustomPlugin was initialized", ConsoleMessages);
    }

    [System.Text.RegularExpressions.GeneratedRegex(".+")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}
