using System.Text.Json.Serialization;

namespace CKEditor.Blazor.Model;

/// <summary>
/// Represents a custom CKEditor plugin loaded from a JavaScript module path.
/// Serialized as <c>{ "$import": { "name": "PluginName", "path": "./module.js" } }</c>.
/// Create instances via <see cref="Plugin.Import"/>.
/// </summary>
[JsonConverter(typeof(Serialization.PresetPluginImportJsonConverter))]
public sealed record PresetPluginImport(string Name, string ImportPath);
