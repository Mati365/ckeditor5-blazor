using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Bundle.SelfHosted;

public interface ICKBoxSelfHostedBundleBuilder
{
    AssetsBundle Build(string version, IReadOnlyList<string> translations, string basePath, string theme = "lark");
}
