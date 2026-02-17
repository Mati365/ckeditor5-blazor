using CKEditor.Blazor.Domain.Model.Bundle;
using CKEditor.Blazor.Domain.Model.SelfHosted;

namespace CKEditor.Blazor.Domain.Interfaces.Bundle.SelfHosted;

public interface ISelfHostedBundleBuilder
{
    AssetsBundle Build(SelfHostedConfig selfHosted);
}
