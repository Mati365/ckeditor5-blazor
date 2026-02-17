using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Bundle.SelfHosted;

public interface ICKEditorSelfHostedBundleBuilder
{
    AssetsBundle Build(string version, string basePath);
}
