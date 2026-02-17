using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.Cloud;

namespace CKEditor.Blazor.Services.Bundle.Cloud;

public interface ICloudBundleBuilder
{
    AssetsBundle Build(CloudConfig cloud);
}
