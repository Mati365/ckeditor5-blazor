namespace CKEditor.Blazor.Exceptions;

/// <summary>
/// Thrown when a preset name cannot be resolved by <see cref="Services.ConfigManager"/>.
/// </summary>
public class UnknownPresetException : CKEditorException
{
    public UnknownPresetException(string presetName)
        : base($"Unknown preset: {presetName}")
    {
    }
}
