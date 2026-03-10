namespace CKEditor.Demo.Tests.Infrastructure;

/// <summary>
/// xUnit fixture for managing a single Playwright browser instance across all tests.
/// Each test class that needs browser access should implement IClassFixture<BrowserFixture>
/// and receive the fixture instance via constructor injection.
/// </summary>
public sealed class BrowserFixture : IAsyncLifetime
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL") ?? "http://localhost:5175";

    private IPlaywright _playwright = null!;

    public IBrowser Browser { get; private set; } = null!;

    private static bool IsHeadless =>
        Environment.GetEnvironmentVariable("PLAYWRIGHT_HEADLESS") is not "false";

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = IsHeadless,
            SlowMo = IsHeadless ? 0 : 100,
        });
    }

    public async Task DisposeAsync()
    {
        await Browser.CloseAsync();
        _playwright.Dispose();
    }
}
