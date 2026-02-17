using CKEditor.Blazor.Model.Bundle;
using CKEditor.Blazor.Model.SelfHosted;

namespace CKEditor.Blazor.Services.Bundle.SelfHosted;

public interface ISelfHostedBundleBuilder
{
    AssetsBundle Build(SelfHostedConfig selfHosted);
}
