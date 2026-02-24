namespace CKEditor.Blazor.Exceptions;

/// <summary>
/// Base class for all exceptions thrown by the CKEditor.Blazor library.
/// Developers can catch this type if they only care about errors originating
/// from the package, without pulling in unrelated system exceptions.
/// </summary>
public class CKEditorException : Exception
{
    public CKEditorException()
    {
    }

    public CKEditorException(string message)
        : base(message)
    {
    }

    public CKEditorException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
