namespace CKEditor.Blazor.Exceptions;

/// <summary>
/// Represents an error in the cloud configuration section of a preset.
/// </summary>
public class CloudConfigurationException : ConfigurationException
{
    public CloudConfigurationException()
    {
    }

    public CloudConfigurationException(string message)
        : base(message)
    {
    }

    public CloudConfigurationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
