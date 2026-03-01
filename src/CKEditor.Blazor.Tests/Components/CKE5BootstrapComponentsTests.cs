using CKEditor.Blazor.Components;

namespace CKEditor.Blazor.Tests.Components;

public class CKE5BootstrapComponentsTests : BunitContext
{
    [Fact]
    public void RendersModuleScript_WithoutNonce()
    {
        var cut = Render<CKE5BootstrapComponents>();

        cut.MarkupMatches("""
            <script type="module">
                import { ensureEditorElementsRegistered } from 'ckeditor5-blazor';

                ensureEditorElementsRegistered();
            </script>
            """);
    }

    [Fact]
    public void RendersModuleScript_WithNonce()
    {
        var cut = Render<CKE5BootstrapComponents>(p => p.Add(p => p.Nonce, "test-nonce"));

        cut.MarkupMatches("""
            <script type="module" nonce="test-nonce">
                import { ensureEditorElementsRegistered } from 'ckeditor5-blazor';

                ensureEditorElementsRegistered();
            </script>
            """);
    }
}
