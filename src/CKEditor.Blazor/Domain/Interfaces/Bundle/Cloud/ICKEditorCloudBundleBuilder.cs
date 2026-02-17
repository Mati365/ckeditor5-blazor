using CKEditor.Blazor.Domain.Model.Bundle;

namespace CKEditor.Blazor.Domain.Interfaces.Bundle.Cloud;

public interface ICKEditorCloudBundleBuilder
{
    AssetsBundle Build(string version, string cdnUrl);
}
