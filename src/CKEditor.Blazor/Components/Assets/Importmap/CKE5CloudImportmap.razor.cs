using CKEditor.Blazor.Model;
using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;
using Microsoft.AspNetCore.Components;

namespace CKEditor.Blazor.Components.Assets;

/// <summary>
/// CKEditor 5 Cloud Import Map Component.
/// Renders only the <c>&lt;script type="importmap"&gt;</c> tag for CKEditor Cloud integration.
/// Use this component in a shared layout or <c>&lt;head&gt;</c> template while placing
/// <c>&lt;CKE5Cloud EmitImportMap="false" EmitModulePreload="false" /&gt;</c> on individual pages
/// that need styles and other per-page assets.
/// </summary>
public partial class CKE5CloudImportmap
{
    [Inject]
    private ICloudBundleBuilder CloudBundleBuilder { get; set; } = default!;

    protected override AssetsBundle? BuildBundle(PresetConfig preset)
    {
        var cloud = preset.EnsureCloudCompatibilityOrThrow();
        return CloudBundleBuilder.Build(cloud);
    }
}
