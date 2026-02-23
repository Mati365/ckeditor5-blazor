using CKEditor.Blazor.Model.Bundle;

namespace CKEditor.Blazor.Services.Interfaces.Bundle.Cloud;

public interface ICKEditorPremiumCloudBundleBuilder
{
    AssetsBundle Build(string version, string cdnUrl);
}
