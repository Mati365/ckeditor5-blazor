using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;

public interface ICKBoxCloudBundleBuilder
{
    AssetsBundle Build(string version, IReadOnlyList<string> translations, string cdnUrl, string theme = "theme");
}
