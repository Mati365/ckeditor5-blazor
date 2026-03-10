namespace CKEditor.Demo.Tests.Infrastructure;

/// <summary>
/// Base class for Playwright page tests.
/// InitializeAsync / DisposeAsync are called by xunit before and after each individual test,
/// so every test gets a fresh browser page.
/// The BrowserFixture (shared browser instance) is injected via IClassFixture on the concrete class.
/// </summary>
public abstract class PageTestBase : IAsyncLifetime
{
    private readonly BrowserFixture _fixture;

    protected IPage Page { get; private set; } = null!;

    protected readonly List<string> ConsoleMessages = [];

    protected abstract string PagePath { get; }

    protected PageTestBase(BrowserFixture fixture) => _fixture = fixture;

    public virtual async Task InitializeAsync()
    {
        Page = await _fixture.Browser.NewPageAsync();
        Page.Console += (_, msg) => ConsoleMessages.Add(msg.Text);
        await OnPageCreatedAsync(Page);
        await Page.GotoAsync($"{BrowserFixture.BaseUrl}{PagePath}");
    }

    protected virtual Task OnPageCreatedAsync(IPage page) => Task.CompletedTask;

    public virtual async Task DisposeAsync() => await Page.CloseAsync();

    protected async Task WaitForEditorAsync(int index = 0)
    {
        var wrapper = Page.Locator("cke5-editor[data-cke-interactive='true']").Nth(index);

        await wrapper.WaitForAsync(new() { State = WaitForSelectorState.Attached, Timeout = 30_000 });
        await wrapper.Locator(".ck-editor__editable[contenteditable='true']").First
            .WaitForAsync(new() { State = WaitForSelectorState.Attached, Timeout = 30_000 });
    }

    protected async Task WaitForEditableAsync(int index = 0)
    {
        var wrapper = Page.Locator("cke5-editable[data-cke-interactive='true']").Nth(index);

        await wrapper.WaitForAsync(new() { State = WaitForSelectorState.Attached, Timeout = 30_000 });
        await wrapper.Locator(".ck-editor__editable[contenteditable='true']").First
            .WaitForAsync(new() { State = WaitForSelectorState.Attached, Timeout = 30_000 });
    }
}
