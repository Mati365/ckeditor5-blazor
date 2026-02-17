using CKEditor.Blazor.Domain.Model.Bundle;

namespace CKEditor.Blazor.Domain.Interfaces.Bundle.Cloud;

public interface ICKBoxCloudBundleBuilder
{
    AssetsBundle Build(string version, IReadOnlyList<string> translations, string cdnUrl, string theme = "theme");
}
