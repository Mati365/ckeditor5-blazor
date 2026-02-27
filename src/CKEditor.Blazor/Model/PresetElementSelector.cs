using System.Text.Json.Serialization;

namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents a selector for identifying elements to which editor configuration should be applied.
/// Serialized as <c>{ "$element": "selector" }</c>.
/// </summary>
[JsonConverter(typeof(Serialization.PresetElementSelectorJsonConverter))]
public sealed record PresetElementSelector(string Selector)
{
}
