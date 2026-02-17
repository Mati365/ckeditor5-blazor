using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Bundle.Cloud;

public interface ICKEditorPremiumCloudBundleBuilder
{
    AssetsBundle Build(string version, string cdnUrl);
}
