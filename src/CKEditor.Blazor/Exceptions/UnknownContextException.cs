namespace CKEditor.Blazor.Exceptions;

/// <summary>
/// Thrown when a context name cannot be resolved by <see cref="Services.ConfigManager"/>.
/// </summary>
public class UnknownContextException : CKEditorException
{
    public UnknownContextException(string contextName)
        : base($"Unknown context: {contextName}")
    {
    }
}
