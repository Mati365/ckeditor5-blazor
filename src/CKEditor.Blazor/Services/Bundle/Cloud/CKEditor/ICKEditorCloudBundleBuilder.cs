using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Bundle.Cloud;

public interface ICKEditorCloudBundleBuilder
{
    AssetsBundle Build(string version, string cdnUrl);
}
