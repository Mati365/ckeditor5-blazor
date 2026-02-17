using CKEditor.Blazor.Domain.Model.Bundle;
using CKEditor.Blazor.Domain.Model.Cloud;

namespace CKEditor.Blazor.Domain.Interfaces.Bundle.Cloud;

public interface ICloudBundleBuilder
{
    AssetsBundle Build(CloudConfig cloud);
}
