namespace CKEditor.Blazor.Exceptions;

/// <summary>
/// Raised when a license key associated with a preset is not compatible with
/// CKEditor Cloud CDN hosting.
/// </summary>
public class CloudLicenseIncompatibleException : CKEditorException
{
    public CloudLicenseIncompatibleException(string message)
        : base(message)
    {
    }

    public CloudLicenseIncompatibleException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
