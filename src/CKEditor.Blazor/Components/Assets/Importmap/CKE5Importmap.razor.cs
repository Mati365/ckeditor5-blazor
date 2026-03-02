using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.License;
using CKEditor.Blazor.Services;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;
using CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components.Assets;

/// <summary>
/// CKEditor 5 Import Map Component.
/// Renders only the <c>&lt;script type="importmap"&gt;</c> tag.
/// Use this component in a shared layout or <c>&lt;head&gt;</c> template while placing
/// <c>&lt;CKE5Assets Distribution="..." EmitImportMap="false" EmitModulePreload="false" /&gt;</c> on individual pages
/// that need styles and other per-page assets.
/// </summary>
public partial class CKE5Importmap : ComponentBase
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

    private Dictionary<string, string> ImportMap
    {
        get
        {
            var importMap = Bundle?.GetImportMap() ?? [];

            foreach (var (key, value) in CustomImportMap)
            {
                importMap[key] = value;
            }

            return importMap;
        }
    }

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
