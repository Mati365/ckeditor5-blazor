using CKEditor.Blazor.Model.SelfHosted;
using CKEditor.Blazor.Services;
using CKEditor.Demo.Server.Components;
using CKEditor.Demo.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCKEditor(options =>
{
    options.Presets["default"] = ConfigManager.CreateDefaultPreset(
        selfHostedConfig: new SelfHostedConfig
        {
            AssetsBasePath = "/_content/CKEditor.Demo.RCL"
        });
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        // Increase SignalR message size to allow large image payloads sent via JS interop.
        // Default is 32 KB which is easily exceeded by Base64-encoded images.
        options.MaxBufferedUnacknowledgedRenderBatches = 10;
    });

builder.Services.AddSignalR(options =>
{
    // Allow up to 50 MB for a single SignalR message so that image uploads
    // routed through Blazor Server's JS interop do not disconnect the circuit.
    options.MaximumReceiveMessageSize = 50 * 1024 * 1024;
});

builder.Services.AddSingleton<ImageStorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/api/images/{id}", (string id, ImageStorageService storage) =>
{
    var image = storage.Get(id);

    if (image is null)
    {
        return Results.NotFound();
    }

    return Results.File(image.Data, image.MimeType, image.FileName);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(CKEditor.Demo.RCL._Imports).Assembly);

app.Run();
