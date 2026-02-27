using System.Text.Json.Serialization;

namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents a reference to a translation key for editor configuration.
/// Serialized as <c>{ "$translation": "key" }</c>.
/// </summary>
[JsonConverter(typeof(Serialization.PresetTranslationReferenceJsonConverter))]
public sealed record PresetTranslationReference(string Key)
{
}
