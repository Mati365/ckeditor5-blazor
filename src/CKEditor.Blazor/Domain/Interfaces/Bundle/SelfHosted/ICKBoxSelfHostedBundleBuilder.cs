using CKEditor.Blazor.Domain.Model.Bundle;

namespace CKEditor.Blazor.Domain.Interfaces.Bundle.SelfHosted;

public interface ICKBoxSelfHostedBundleBuilder
{
    AssetsBundle Build(string version, IReadOnlyList<string> translations, string basePath, string theme = "lark");
}
