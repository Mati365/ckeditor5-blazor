namespace CKEditor.Blazor.Exceptions;

/// <summary>
/// Raised when a license key associated with a preset is not compatible with
/// self-hosted CKEditor usage.
/// </summary>
public class SelfHostedLicenseIncompatibleException : CKEditorException
{
    public SelfHostedLicenseIncompatibleException(string message)
        : base(message)
    {
    }

    public SelfHostedLicenseIncompatibleException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
