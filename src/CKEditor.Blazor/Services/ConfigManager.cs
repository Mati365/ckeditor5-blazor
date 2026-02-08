using CKEditor.Blazor.Cloud;
using CKEditor.Blazor.Context;
using CKEditor.Blazor.Preset;

namespace CKEditor.Blazor.Services;

/// <summary>
/// Service for managing CKEditor configurations, presets, and contexts.
/// </summary>
public class ConfigManager
{
    private readonly Dictionary<string, PresetConfig> _presets = [];
    private readonly Dictionary<string, ContextConfig> _contexts = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigManager"/> class.
    /// </summary>
    public ConfigManager()
    {
        // Register default preset
        RegisterPreset("default", new PresetConfig
        {
            EditorType = EditorType.Classic,
            Cloud = new CloudConfig
            {
                EditorVersion = "47.3.0",
                Premium = false,
                Translations = ["en"]
            },
            Config = new Dictionary<string, object>
            {
                ["toolbar"] = new Dictionary<string, object>
                {
                    ["items"] = new object[]
                    {
                        "undo",
                        "redo",
                        "|",
                        "heading",
                        "|",
                        "fontFamily",
                        "fontSize",
                        "fontColor",
                        "fontBackgroundColor",
                        "alignment",
                        "|",
                        "bold",
                        "italic",
                        "underline",
                        new Dictionary<string, object>
                        {
                            ["label"] = "Text Style",
                            ["items"] = new[] { "strikethrough", "superscript", "subscript" }
                        },
                        "|",
                        "link",
                        "insertImage",
                        "insertTable",
                        "insertTableLayout",
                        "blockQuote",
                        "emoji",
                        "mediaEmbed",
                        "|",
                        "bulletedList",
                        "numberedList",
                        "todoList",
                        "outdent",
                        "indent"
                    }
                },
                ["language"] = "en",
                ["plugins"] = new[]
                {
                    "Alignment",
                    "AccessibilityHelp",
                    "Autoformat",
                    "AutoImage",
                    "Autosave",
                    "BlockQuote",
                    "Bold",
                    "CloudServices",
                    "Essentials",
                    "Emoji",
                    "Mention",
                    "Heading",
                    "FontFamily",
                    "FontSize",
                    "FontColor",
                    "FontBackgroundColor",
                    "ImageBlock",
                    "ImageCaption",
                    "ImageInline",
                    "ImageInsert",
                    "ImageInsertViaUrl",
                    "ImageResize",
                    "ImageStyle",
                    "ImageTextAlternative",
                    "ImageToolbar",
                    "ImageUpload",
                    "Indent",
                    "IndentBlock",
                    "Italic",
                    "Link",
                    "LinkImage",
                    "List",
                    "ListProperties",
                    "MediaEmbed",
                    "Paragraph",
                    "PasteFromOffice",
                    "PictureEditing",
                    "SelectAll",
                    "Table",
                    "TableLayout",
                    "TableCaption",
                    "TableCellProperties",
                    "TableColumnResize",
                    "TableProperties",
                    "TableToolbar",
                    "TextTransformation",
                    "TodoList",
                    "Underline",
                    "Strikethrough",
                    "Superscript",
                    "Subscript",
                    "Undo",
                    "Base64UploadAdapter"
                },
                ["table"] = new Dictionary<string, object>
                {
                    ["contentToolbar"] = new[]
                    {
                        "tableColumn",
                        "tableRow",
                        "mergeTableCells",
                        "tableProperties",
                        "tableCellProperties",
                        "toggleTableCaption"
                    }
                },
                ["image"] = new Dictionary<string, object>
                {
                    ["toolbar"] = new[]
                    {
                        "imageTextAlternative",
                        "imageStyle",
                        "imageResize",
                        "imageInsertViaUrl"
                    }
                }
            }
        });

        // Register default context
        RegisterContext("default", new ContextConfig
        {
            Config = []
        });
    }

    /// <summary>
    /// Registers a preset with the specified name.
    /// </summary>
    /// <param name="name">The name of the preset.</param>
    /// <param name="preset">The preset to register.</param>
    public void RegisterPreset(string name, PresetConfig preset)
    {
        _presets[name] = preset;
    }

    /// <summary>
    /// Registers a context with the specified name.
    /// </summary>
    /// <param name="name">The name of the context.</param>
    /// <param name="context">The context to register.</param>
    public void RegisterContext(string name, ContextConfig context)
    {
        _contexts[name] = context;
    }

    /// <summary>
    /// Resolves a preset by name or returns the provided preset object.
    /// </summary>
    /// <param name="preset">The preset name or preset object.</param>
    /// <returns>The resolved preset.</returns>
    public PresetConfig ResolvePreset(object? preset)
    {
        return preset switch
        {
            null => _presets.GetValueOrDefault("default") ?? new PresetConfig(),
            string presetName => _presets.GetValueOrDefault(presetName) ?? throw new InvalidOperationException($"Unknown preset: {presetName}"),
            PresetConfig presetObj => presetObj,
            _ => throw new ArgumentException("Invalid preset type", nameof(preset))
        };
    }

    /// <summary>
    /// Resolves a preset by name or throws an exception if not found.
    /// </summary>
    /// <param name="presetName">The name of the preset.</param>
    /// <returns>The resolved preset.</returns>
    public PresetConfig ResolvePresetOrThrow(string presetName)
    {
        if (!_presets.TryGetValue(presetName, out var preset))
        {
            throw new InvalidOperationException($"Unknown preset: {presetName}");
        }

        return preset;
    }

    /// <summary>
    /// Resolves a context by name or returns the provided context object.
    /// </summary>
    /// <param name="context">The context name or context object.</param>
    /// <returns>The resolved context.</returns>
    public ContextConfig ResolveContext(object? context)
    {
        return context switch
        {
            null => _contexts.GetValueOrDefault("default") ?? new ContextConfig(),
            string contextName => _contexts.GetValueOrDefault(contextName) ?? throw new InvalidOperationException($"Unknown context: {contextName}"),
            ContextConfig contextObj => contextObj,
            _ => throw new ArgumentException("Invalid context type", nameof(context))
        };
    }

    /// <summary>
    /// Resolves a context by name or throws an exception if not found.
    /// </summary>
    /// <param name="contextName">The name of the context.</param>
    /// <returns>The resolved context.</returns>
    public ContextConfig ResolveContextOrThrow(string contextName)
    {
        if (!_contexts.TryGetValue(contextName, out var context))
        {
            throw new InvalidOperationException($"Unknown context: {contextName}");
        }

        return context;
    }

    /// <summary>
    /// Gets all registered presets.
    /// </summary>
    /// <returns>A read-only dictionary of all registered presets.</returns>
    public IReadOnlyDictionary<string, PresetConfig> GetPresets()
    {
        return _presets;
    }

    /// <summary>
    /// Gets all registered contexts.
    /// </summary>
    /// <returns>A read-only dictionary of all registered contexts.</returns>
    public IReadOnlyDictionary<string, ContextConfig> GetContexts()
    {
        return _contexts;
    }
}
