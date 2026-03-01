using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components.Assets;

/// <summary>
/// CKEditor 5 Self-Hosted Import Map Component.
/// Renders only the <c>&lt;script type="importmap"&gt;</c> tag for self-hosted CKEditor integration.
/// Use this component in a shared layout or <c>&lt;head&gt;</c> template while placing
/// <c>&lt;CKE5SelfHosted EmitImportMap="false" EmitModulePreload="false" /&gt;</c> on individual pages
/// that need styles and other per-page assets.
/// </summary>
public partial class CKE5SelfHostedImportmap
{
    [Inject]
    private ISelfHostedBundleBuilder SelfHostedBundleBuilder { get; set; } = default!;

    protected override AssetsBundle? BuildBundle(PresetConfig preset)
    {
        var selfHosted = preset.EnsureSelfHostedCompatibilityOrThrow();
        return SelfHostedBundleBuilder.Build(selfHosted);
    }
}
