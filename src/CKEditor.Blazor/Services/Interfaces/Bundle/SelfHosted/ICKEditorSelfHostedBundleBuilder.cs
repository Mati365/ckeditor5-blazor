using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Interfaces.Bundle.SelfHosted;

public interface ICKEditorSelfHostedBundleBuilder
{
    AssetsBundle Build(string version, string basePath);
}
