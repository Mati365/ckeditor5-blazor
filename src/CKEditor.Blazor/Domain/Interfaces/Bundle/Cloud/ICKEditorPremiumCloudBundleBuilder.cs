using CKEditor.Blazor.Domain.Model.Bundle;

namespace CKEditor.Blazor.Domain.Interfaces.Bundle.Cloud;

public interface ICKEditorPremiumCloudBundleBuilder
{
    AssetsBundle Build(string version, string cdnUrl);
}
