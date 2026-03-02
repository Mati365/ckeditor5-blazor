using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.License;
using CKEditor.Blazor.Services;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;
using CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components.Assets;

/// <summary>
/// CKEditor 5 Assets Component.
/// Renders the necessary script and stylesheet tags for CKEditor integration.
/// Use <see cref="Distribution"/> to select between CDN and self-hosted assets.
/// </summary>
public partial class CKE5Assets : ComponentBase
{
    /// <summary>
    /// The distribution channel that determines which bundle builder to use.
    /// Use <see cref="DistributionChannel.Cloud"/> for CDN-hosted assets
    /// or <see cref="DistributionChannel.SH"/> for self-hosted assets.
    /// </summary>
    [Parameter]
    public DistributionChannel Distribution { get; set; } = DistributionChannel.SH;

    /// <summary>
    /// The preset name to use (default: 'default').
    /// </summary>
    [Parameter]
    public string Preset { get; set; } = "default";

    /// <summary>
    /// Optional nonce for CSP (Content Security Policy).
    /// </summary>
    [Parameter]
    public string? Nonce { get; set; }

    /// <summary>
    /// Whether to emit the import map script tag. Default is true.
    /// Set to false if you want to manage the import map yourself (e.g. via <c>&lt;CKE5Importmap /&gt;</c>).
    /// </summary>
    [Parameter]
    public bool EmitImportMap { get; set; } = true;

    /// <summary>
    /// Whether to emit module preload link tags for ESM assets. Default is true.
    /// Set to false when the import map is declared globally (e.g. via <c>&lt;CKE5Importmap /&gt;</c>)
    /// and you only want per-page assets such as stylesheets rendered here.
    /// </summary>
    [Parameter]
    public bool EmitModulePreload { get; set; } = true;

    /// <summary>
    /// Custom import map entries to merge with the generated import map.
    /// </summary>
    [Parameter]
    public Dictionary<string, string> CustomImportMap { get; set; } = [];

    [Inject]
    private ConfigManager ConfigManager { get; set; } = default!;

    [Inject]
    private ICloudBundleBuilder CloudBundleBuilder { get; set; } = default!;

    [Inject]
    private ISelfHostedBundleBuilder SelfHostedBundleBuilder { get; set; } = default!;

    private AssetsBundle? Bundle { get; set; }

    protected override void OnInitialized()
    {
        var preset = ConfigManager.ResolvePresetOrThrow(Preset);

        Bundle = Distribution switch
        {
            DistributionChannel.Cloud => CloudBundleBuilder.Build(preset.EnsureCloudCompatibilityOrThrow()),
            DistributionChannel.SH => SelfHostedBundleBuilder.Build(preset.EnsureSelfHostedCompatibilityOrThrow()),
            _ => throw new InvalidOperationException($"Unsupported distribution channel: {Distribution}.")
        };
    }
}
