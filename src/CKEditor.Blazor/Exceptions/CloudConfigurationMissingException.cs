namespace CKEditor.Blazor.Exceptions;

/// <summary>
/// Thrown when code expects a cloud configuration to be present but the
/// preset does not contain one.
/// </summary>
public class CloudConfigurationMissingException : CloudConfigurationException
{
    public CloudConfigurationMissingException()
        : base($"Cloud configuration is missing for used preset.")
    {
    }
}
