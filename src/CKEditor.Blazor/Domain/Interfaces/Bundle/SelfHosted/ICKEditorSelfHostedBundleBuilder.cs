using CKEditor.Blazor.Domain.Model.Bundle;

namespace CKEditor.Blazor.Domain.Interfaces.Bundle.SelfHosted;

public interface ICKEditorSelfHostedBundleBuilder
{
    AssetsBundle Build(string version, string basePath);
}
