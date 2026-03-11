using CKEditor.Demo.Tests.Infrastructure;

namespace CKEditor.Demo.Tests.Tests;

public class ContextEditorTests : PageTestBase, IClassFixture<BrowserFixture>
{
    protected override string PagePath => "/context";

    public ContextEditorTests(BrowserFixture fixture) : base(fixture) { }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await WaitForEditorAsync(index: 0);
        await WaitForEditorAsync(index: 1);
    }

    [Fact]
    public async Task Context_RendersTwoInteractiveEditors()
    {
        var editors = Page.Locator("cke5-editor[data-cke-interactive='true']");
        await Assertions.Expect(editors).ToHaveCountAsync(2);
    }

    [Fact]
    public async Task FirstEditor_ContainsInitialContent()
    {
        var editable = Page
            .Locator("cke5-editor")
            .Nth(0)
            .Locator(".ck-editor__editable");

        await Assertions.Expect(editable).ToContainTextAsync("Editor 1 content");
    }

    [Fact]
    public async Task SecondEditor_ContainsInitialContent()
    {
        var editable = Page
            .Locator("cke5-editor")
            .Nth(1)
            .Locator(".ck-editor__editable");

        await Assertions.Expect(editable).ToContainTextAsync("Editor 2 content");
    }

    [Fact]
    public async Task BothEditors_ShareTheSameContextId()
    {
        var editor1 = Page.Locator("cke5-editor").Nth(0);
        var editor2 = Page.Locator("cke5-editor").Nth(1);

        var contextAttr1 = await editor1.GetAttributeAsync("data-cke-context-id");

        Assert.False(string.IsNullOrEmpty(contextAttr1));

        await Assertions.Expect(editor2).ToHaveAttributeAsync("data-cke-context-id", contextAttr1);
    }

    [Fact]
    public void CustomContextPlugin_IsInitialized()
    {
        Assert.Contains("MyCustomContextPlugin was initialized", ConsoleMessages);
    }

    [Fact]
    public void CustomPlugin_IsInitialized()
    {
        Assert.Contains("MyCustomPlugin was initialized", ConsoleMessages);
    }
}
