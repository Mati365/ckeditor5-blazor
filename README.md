# ckeditor5-blazor

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-green.svg?style=flat-square)](http://makeapullrequest.com)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/mati365/ckeditor5-blazor?style=flat-square)
[![GitHub issues](https://img.shields.io/github/issues/mati365/ckeditor5-blazor?style=flat-square)](https://github.com/Mati365/ckeditor5-blazor/issues)
[![TS Coverage](https://img.shields.io/badge/TypeScript-100%25-brightgreen?logo=typescript&logoColor=white&style=flat-square)](https://app.codecov.io/gh/Mati365/ckeditor5-blazor/tree/main/npm_package%2Fsrc)
[![C# Coverage](https://img.shields.io/badge/C%23-100%25-brightgreen?logo=dotnet&logoColor=white&style=flat-square)](https://app.codecov.io/gh/Mati365/ckeditor5-blazor/tree/main/src)
[![NPM Version](https://img.shields.io/npm/v/ckeditor5-blazor?style=flat-square)](https://www.npmjs.com/package/ckeditor5-blazor)
[![NuGet](https://img.shields.io/nuget/v/CKEditor.Blazor?style=flat-square&color=%239245ba)](https://www.nuget.org/packages/CKEditor.Blazor/)

CKEditor 5 for Blazor - a lightweight multiplatform WYSIWYG editor integration for ASP.NET Core Blazor Server and WebAssembly. It works with Razor components and .NET forms. Easy to set up, it supports self-hosted assets, CDN loading, multiple editor types, shared contexts, localization, and custom plugins.

> [!IMPORTANT]
> This integration is unofficial and not maintained by CKSource. For official CKEditor 5 documentation, visit [ckeditor.com](https://ckeditor.com/docs/ckeditor5/latest/). If you encounter any issues in editor, please report them on the [GitHub repository](https://github.com/ckeditor/ckeditor5/issues).

<!-- markdownlint-disable MD033 -->
<p align="center">
  <img src="docs/intro-classic-editor.png" alt="CKEditor 5 Classic Editor in .NET / Blazor application">
</p>

## Table of Contents

- [ckeditor5-blazor](#ckeditor5-blazor)
  - [Table of Contents](#table-of-contents)
  - [Installation 🚀](#installation-)
    - [🏠 Self-hosted via MSBuild](#-self-hosted-via-msbuild)
    - [📡 CDN Distribution](#-cdn-distribution)
  - [Basic Usage 🏁](#basic-usage-)
    - [Simple Editor ✏️](#simple-editor-️)
    - [Static rendering with `Interactive=true` 🧱](#static-rendering-with-interactivetrue-)
  - [Configuration ⚙️](#configuration-️)
    - [Override default preset configuration 🧑‍💻](#override-default-preset-configuration-)
    - [Define your configuration directly in the view 💻](#define-your-configuration-directly-in-the-view-)
    - [Preset DSL 🛠️](#preset-dsl-️)
    - [Define reusable configuration presets 🧩](#define-reusable-configuration-presets-)
    - [Dynamic presets 🎯](#dynamic-presets-)
    - [Element references using `$element` 🎯](#element-references-using-element-)
  - [Providing the License Key 🗝️](#providing-the-license-key-️)
  - [Localization 🌍](#localization-)
    - [Translation Loading 🌐](#translation-loading-)
    - [Global Translation Config 🛠️](#global-translation-config-️)
    - [Custom translations 🌐](#custom-translations-)
    - [Translation references using `$translation` ✨](#translation-references-using-translation-)
  - [Editor Types 🖊️](#editor-types-️)
    - [Classic editor 📝](#classic-editor-)
    - [Inline editor 📝](#inline-editor-)
    - [Balloon editor 🎈](#balloon-editor-)
    - [Decoupled editor 🌐](#decoupled-editor-)
    - [Multiroot editor 🌳](#multiroot-editor-)
  - [Advanced configuration ⚙️](#advanced-configuration-️)
    - [Blazor Data Binding 🔄](#blazor-data-binding-)
      - [Two way binding using `@bind-Value` ⛓️](#two-way-binding-using-bind-value-️)
        - [Multiroot Editables 🌳⛓️](#multiroot-editables-️)
      - [Bidirectional Communication using Events 🔄](#bidirectional-communication-using-events-)
        - [Editor → .NET: Content Change Event 📤](#editor--net-content-change-event-)
        - [.NET → Editor: Set Content 📥](#net--editor-set-content-)
    - [Editor Ready Event ✅](#editor-ready-event-)
    - [Focus Tracking 👁️](#focus-tracking-️)
    - [Image Upload 🖼️](#image-upload-️)
    - [Watchdog 🐶](#watchdog-)
      - [Disabling the watchdog 🚫](#disabling-the-watchdog-)
    - [Splitting Assets: Global Import Map with Per-page Styles 🗺️](#splitting-assets-global-import-map-with-per-page-styles-️)
      - [Self-hosted variant 🏠](#self-hosted-variant-)
      - [CDN variant 📡](#cdn-variant-)
      - [Disabling module preload hints ⏳](#disabling-module-preload-hints-)
  - [Context 🤝](#context-)
    - [Basic usage 🔧](#basic-usage--1)
    - [Custom context config 🌐](#custom-context-config-)
    - [Context config DSL 🛠️](#context-config-dsl-️)
  - [Custom plugins 🧩](#custom-plugins-)
    - [Import from a JS module 📦](#import-from-a-js-module-)
    - [Register in a JS bundle 🗂️](#register-in-a-js-bundle-️)
  - [Editors and Contexts registry 👀](#editors-and-contexts-registry-)
  - [Development ⚙️](#development-️)
    - [Running Tests 🧪](#running-tests-)
      - [Running E2E tests 🧪](#running-e2e-tests-)
  - [Psst... 👀](#psst-)
  - [Trademarks 📜](#trademarks-)
  - [License 📜](#license-)

## Installation 🚀

Choose between two installation methods based on your needs. Both approaches provide the same editor API in Razor, but differ in how CKEditor 5 assets are loaded and managed.

### 🏠 Self-hosted via MSBuild

Bundle CKEditor 5 with your application for full control over assets, versioning, and offline support. During build, the package downloads required assets automatically.

**Complete setup:**

1. **Add NuGet dependency:**

   ```bash
   dotnet add package CKEditor.Blazor
   ```

2. **(Optional) Override MSBuild asset options** in your `.csproj`:

   ```xml
   <PropertyGroup>
     <CKEditorVersion>47.6.0</CKEditorVersion>
     <CKEditorIncludePremiumAssets>false</CKEditorIncludePremiumAssets>
     <CKBoxVersion>2.8.0</CKBoxVersion>
     <CKBoxIncludeAssets>true</CKBoxIncludeAssets>
     <CKEditorAssetsOutputPath>$(MSBuildProjectDirectory)/wwwroot</CKEditorAssetsOutputPath>
   </PropertyGroup>
   ```

3. **Register CKEditor services** in `Program.cs`:

   ```csharp
   using CKEditor.Blazor.Services;

   builder.Services.AddCKEditor();
   ```

   By default the package infers the correct asset URL from build metadata, so no extra configuration is needed for the typical setup.

   If your static files are served from a non-standard base path (e.g. behind a reverse proxy with a path prefix, or assets placed in a subdirectory), set `AssetsBasePath` to match the actual URL prefix:

   ```csharp
   using CKEditor.Blazor.Model.SelfHosted;
   using CKEditor.Blazor.Services;

   builder.Services.AddCKEditor(options => options
       .ExtendDefaultPreset(preset => preset
            .WithSelfHosted(new SelfHostedConfig
            {
                AssetsBasePath = "/static/ckeditor"
            })));
   ```

   The value is the URL prefix (without a trailing slash) prepended to all generated asset URLs. It must match what the browser actually uses to fetch the files.

4. **Build your project** to download and prepare assets:

   ```bash
   dotnet build
   ```

5. **Add self-hosted assets component** in `<head>` (e.g. `App.razor`):

   ```razor
   @using CKEditor.Blazor.Components.Assets

   <HeadContent>
       <CKE5Assets />
   </HeadContent>
   ```

6. **Use editor components** anywhere in your Razor UI:

   ```razor
   @using CKEditor.Blazor.Components

   <CKE5Editor Value="@("<p>Hello world!</p>")" />
   ```

### 📡 CDN Distribution

Load CKEditor 5 from CKSource CDN using import maps. This method avoids local asset downloads and is good for quick setup.

**Complete setup:**

1. **Add NuGet dependency:**

   ```bash
   dotnet add package CKEditor5.Blazor
   ```

2. (Optional) **Override MSBuild asset options** in your `.csproj`:

   ```xml
   <PropertyGroup>
     <CKEditorIncludeAssets>false</CKEditorIncludeAssets>
     <CKEditorIncludePremiumAssets>false</CKEditorIncludePremiumAssets>
     <CKBoxIncludeAssets>false</CKBoxIncludeAssets>
   </PropertyGroup>
   ```

3. **Build your project** to download and prepare assets:

   ```bash
   dotnet build
   ```

4. **Register CKEditor with cloud preset** in `Program.cs`:

   ```csharp
   using CKEditor.Blazor.Model.Cloud;
   using CKEditor.Blazor.Services;

   builder.Services.AddCKEditor(options => options
        .SetLicenseKey("your-license-key-here")
        .ExtendDefaultPreset(preset => preset
            .WithCloud(new CloudConfig
            {
                EditorVersion = "47.6.0",
                Premium = false
            })));
   ```

5. **Add cloud assets component** in `<head>`:

   ```razor
   @using CKEditor.Blazor.Components.Assets
   @using CKEditor.Blazor.Model.License

   <HeadContent>
       <CKE5Assets Distribution="DistributionChannel.Cloud" />
   </HeadContent>
   ```

6. **Use editor components** anywhere in your Razor UI:

   ```razor
   @using CKEditor.Blazor.Components

   <CKE5Editor Value="@("<p>Hello world!</p>")" />
   ```

That's it! 🎉

## Basic Usage 🏁

Get started with the most common usage pattern. This example shows how to render an editor in Razor and keep content synced with .NET state.

### Simple Editor ✏️

Create a basic editor with default toolbar and plugins.

```razor
@using CKEditor.Blazor.Components
@using CKEditor.Blazor.Model

<CKE5Editor
    EditorType="EditorType.Classic"
    Value="@("<p>Initial content</p>")"
    EditableHeight="300"
    @bind-Value="content" />

@code {
    private EditorValue content = "<p>Initial content</p>";
}
```

`EditorValue` can be initialized from a plain string (mapped to the `main` root) or from a `Dictionary<string, string>` for multi-root editors, where each key is a root name:

```razor
@using CKEditor.Blazor.Components
@using CKEditor.Blazor.Model

<CKE5Editor
    EditorType="EditorType.Multiroot"
    @bind-Value="content" />

@code {
    private EditorValue content = new Dictionary<string, string>
    {
        ["header"] = "<p>Header content</p>",
        ["main"]   = "<p>Main content</p>",
        ["footer"] = "<p>Footer content</p>"
    };
}
```

### Static rendering with `Interactive=true` 🧱

By default, components initialize through Blazor .NET interop callbacks.
If your page is rendered in a non-interactive/static mode, those callbacks are not available.

Set `Interactive="true"` to let CKEditor web components bootstrap directly in the browser without .NET interop initialization:

```razor
@using CKEditor.Blazor.Model

<CKE5Editor
    EditorType="EditorType.Classic"
    Interactive="true"
    Value="@("<p>Static page content</p>")" />
```

This is useful when rendering static pages where you still want the editor UI to initialize on the client.

## Configuration ⚙️

You can configure editor presets in `AddCKEditor(...)`. The default preset is `default`. Presets are reusable configuration objects that can be applied to any editor instance. You can also define custom presets and override the default one.

### Override default preset configuration 🧑‍💻

You can pass initial content and merge additional configuration. In scenario below, the `MergeConfig` will extend the `default` preset configuration to make the menu bar visible. It's only shallow merge, so nested arrays will be replaced, not merged.

```razor
<CKE5Editor
    Value="<p>This is the initial content of the editor.</p>"
    MergeConfig="@(new Dictionary<string, object>
    {
        ["menuBar"] = new Dictionary<string, object>
        {
            ["isVisible"] = true
        }
    })" />
```

Alternatively, you can extend the default configuration directly in `Program.cs` when registering services:

```csharp
builder.Services.AddCKEditor(options =>
    options.ExtendDefaultPreset(preset => preset
        .WithMergedConfig(new Dictionary<string, object>
        {
            ["menuBar"] = new Dictionary<string, object>
            {
                ["isVisible"] = true
            }
        })));
```

### Define your configuration directly in the view 💻

Override the default configuration with custom plugins and toolbar items. In this example, the editor will only have `Essentials`, `Paragraph`, `Bold`, `Italic`, `Link`, and `Undo` plugins, and the toolbar will contain only bold, italic, link, undo, and redo buttons. The editor locale is set to Polish (`pl`), and a custom translation for the "Bold" label is provided.

```razor
<CKE5Editor
    Language="@("pl")"
    Config="@(new Dictionary<string, object>
    {
        ["plugins"] = new[] { "Essentials", "Paragraph", "Bold", "Italic", "Link", "Undo" },
        ["toolbar"] = new Dictionary<string, object>
        {
            ["items"] = new[] { "bold", "italic", "link", "|", "undo", "redo" }
        }
    })" />
```

In order to specify the `UI` and `Content` language separately, use the `Language` object:

```razor
@using CKEditor.Blazor.Model

<CKE5Editor Language="@(new Language { UI = "pl", Content = "en" })" />
```

### Preset DSL 🛠️

If you prefer strongly typed, fluent configuration over raw dictionaries, build presets with `PresetConfig` and register them through `CKEditorOptions`:

```csharp
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Services;

builder.Services.AddCKEditor(options => options
    .SetLicenseKey("GPL")
    .AddDefaultPreset(preset => preset
        .WithEditorType(EditorType.Classic)
        .WithPlugins("Essentials", "Paragraph", "Bold", "Italic", "Undo")
        .WithToolbar("bold", "italic", Toolbar.Separator, "undo", "redo")
        .WithLanguage("pl")
        .WithCustomTranslations("pl", new Dictionary<string, string>
        {
            ["Bold"] = "Pogrubienie"
        })));
```

### Define reusable configuration presets 🧩

In order to override the default preset or add custom presets, use the fluent `AddPreset` helper:

```csharp
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Services;

builder.Services.AddCKEditor(options => options
    .AddPreset("minimal", preset => preset
        .WithEditorType(EditorType.Classic)
        .WithPlugins("Essentials", "Paragraph", "Bold", "Italic", "Undo")
        .WithToolbar("bold", "italic", Toolbar.Separator, "undo", "redo")));
```

Use it in Razor:

```razor
<CKE5Editor Preset="@("minimal")" Value="<p>Simple editor</p>" />
```

### Dynamic presets 🎯

You can also create dynamic presets that can be modified at runtime. This is useful if you want to change the editor configuration based on user input or other conditions.

```razor
@using CKEditor.Blazor.Model
@using CKEditor.Blazor.Services

<CKE5Editor Preset="@dynamicPreset" Value="<p>Runtime preset</p>" />

@code {
    private readonly PresetConfig dynamicPreset = ConfigManager.CreateDefaultPreset()
        .WithToolbar("bold", "italic", "link", Toolbar.Separator, "undo", "redo");
}
```

### Element references using `$element` 🎯

Similarly to translation references, configuration objects may reference DOM elements by CSS selector. Use `PresetConfig.ElementSelector` anywhere in your editor configuration where CKEditor expects an `HTMLElement`, and the package will resolve it to the matching DOM element during initialization (serializes to `{ "$element": "selector" }`).

This is useful, for example, when pointing a plugin to an external container element:

```csharp
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Services;

builder.Services.AddCKEditor(options => options
    .ExtendDefaultPreset(preset => preset
        .WithConfigEntry("myPlugin", new Dictionary<string, object>
        {
            ["container"] = PresetConfig.ElementSelector("#my-container")
        })));
```

If no element matching the selector is found in the DOM, a warning is printed and `null` is used instead.

## Providing the License Key 🗝️

CKEditor 5 requires a license key for official CDN and premium features.

1. **Environment variable** (recommended for production):

   ```bash
   export CKEditor__DefaultLicenseKey="your-license-key-here"
   ```

2. **Programmatic config** in `Program.cs`:

   ```csharp
   builder.Services.AddCKEditor(options => options
       .SetLicenseKey("your-license-key-here"));
   ```

If you use CKEditor 5 under GPL, use `GPL` as your key value.

## Localization 🌍

Support multiple languages in the editor UI and content. Configure translation loading, custom dictionaries, and reuse translation keys or DOM element references across your configuration.

### Translation Loading 🌐

For self-hosted setups, translation assets are handled by your bundler automatically. For cloud setups, translations are loaded through the configured CDN bundle. In both cases, set the UI language per editor or context:

```razor
<CKE5Editor
    Language="@("pl")"
    Value="<p>Treść z polskim UI</p>" />
```

### Global Translation Config 🛠️

Set default language and translated labels in your preset configuration:

```csharp
builder.Services.AddCKEditor(options => options
    .ExtendDefaultPreset(preset => preset
        .WithLanguage("pl")
        .WithCustomTranslations("pl", new Dictionary<string, string>
        {
            ["Bold"] = "Pogrubienie",
            ["Italic"] = "Kursywa",
            ["Link"] = "Link",
            ["Undo"] = "Cofnij",
            ["Redo"] = "Ponów"
        })));
```

### Custom translations 🌐

You can override translations per editor instance via `CustomTranslations`:

```razor
@using CKEditor.Blazor.Model

<CKE5Editor
    Value="<p>Custom labels</p>"
    CustomTranslations="@(new EditorTranslations
    {
        ["pl"] = new Dictionary<string, string>
        {
            ["Bold"] = "Pogrubienie (custom)"
        }
    })" />
```

### Translation references using `$translation` ✨

In addition to supplying full translation maps, configuration objects may contain reference helpers that point to existing translation keys. This is particularly handy when you want to reuse an existing label or avoid repeating the same string in multiple places. Use `PresetConfig.TranslationReference` in any part of your editor or context configuration, and the package will automatically replace it with the correct localized string during initialization (serializes to `{ "$translation": "key" }`).

```csharp
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Services;

builder.Services.AddCKEditor(options => options
    .ExtendDefaultPreset(preset => preset
        .WithCustomTranslations("pl", new Dictionary<string, string>
        {
            ["Bold"] = "Pogrubienie"
        })
        .WithConfigEntry("myPlugin", new Dictionary<string, object>
        {
            ["buttonLabel"] = PresetConfig.TranslationReference("Bold")
        })));
```

When the editor or context is created, the helper will be resolved against the loaded translations (including any custom translations you provided). If the key is not found, a warning is printed and `null` will be used instead.

## Editor Types 🖊️

CKEditor 5 for Blazor supports four distinct editor types, each designed for specific use cases. Choose the one that best fits your application's layout and functionality requirements.

### Classic editor 📝

Traditional WYSIWYG editor with a fixed toolbar above the editing area. Best for standard content editing scenarios like blog posts, articles, or forms.

![CKEditor 5 Classic Editor in Blazor application](docs/classic.png)

```razor
@using CKEditor.Blazor.Model

<CKE5Editor
    EditorType="EditorType.Classic"
    Value="@("<p>This is the initial content of the editor.</p>")"
    EditableHeight="300" />
```

### Inline editor 📝

Minimalist editor that appears directly within content when clicked. Ideal for in-place editing scenarios where the editing interface should be invisible until needed.

![CKEditor 5 Inline Editor in Blazor application](docs/inline-editor.png)

```razor
@using CKEditor.Blazor.Model

<CKE5Editor
    EditorType="EditorType.Inline"
    Value="@("<p>Inline editor content</p>")"
    Class="border border-gray-300" />
```

> [!NOTE]
> Inline editors don't work with `<textarea>` elements and may not be suitable for traditional form scenarios.

### Balloon editor 🎈

Contextual editor that shows a floating toolbar near the selected text. Great for comment editing, annotations, or any scenario where a non-intrusive editing experience is desired.

![CKEditor 5 Balloon Editor in Blazor application](docs/balloon-editor.png)

```razor
@using CKEditor.Blazor.Model

<CKE5Editor
    EditorType="EditorType.Balloon"
    Value="<p>Balloon editor content</p>"
    Class="border border-gray-300" />
```

### Decoupled editor 🌐

Flexible editor where toolbar and editing area are completely separated. Provides maximum layout control for custom interfaces and complex applications.

![CKEditor 5 Decoupled Editor in Blazor application](docs/decoupled-editor.png)

```razor
@using CKEditor.Blazor.Model

<CKE5Editor EditorType="EditorType.Decoupled">
    <CKE5UIPart Name="toolbar" Class="mb-4" />
    <CKE5Editable
        RootName="main"
        Value="<p>This is the initial content of the decoupled editor editable.</p>"
        InnerClass="p-4" />
</CKE5Editor>
```

> [!NOTE]
> `EditorId` is passed down automatically to `CKE5Editable` and `CKE5UIPart` components via cascading parameters if they are placed inside the `<CKE5Editor>`. If placed outside, you must manually set `EditorId` on each of them to link them to the specific editor. Otherwise, all editables will bind to the first editor instance found in the DOM.

### Multiroot editor 🌳

Advanced editor supporting multiple separate editing areas (roots) with a shared toolbar. Perfect for complex documents with multiple editable sections like headers, sidebars, and main content.

![CKEditor 5 Multiroot Editor in Blazor application](docs/multiroot-editor.png)

You can set the content for all roots at once via the `<CKE5Editor>` component using a dictionary. Both `Value` and `@bind-Value` are supported (see [Blazor Data Binding](#blazor-data-binding-)).

```razor
@using CKEditor.Blazor.Model

<CKE5Editor
    EditorType="EditorType.Multiroot"
    Value="@(new Dictionary<string, string>
    {
        ["header"] = "<p>Header content</p>",
        ["content"] = "<p>Main content</p>",
        ["footer"] = "<p>Footer content</p>"
    })">

    <CKE5UIPart Name="toolbar" Class="mb-4" />

    <CKE5Editable RootName="header" />
    <CKE5Editable RootName="content" />
    <CKE5Editable RootName="footer" />
</CKE5Editor>
```

Alternatively, you can provide the initial `Value` or use two-way binding (`@bind-Value`) directly on the individual editable components (see [Multiroot Editables binding](#multiroot-editables-️)):

```razor
@using CKEditor.Blazor.Model

<CKE5Editor EditorType="EditorType.Multiroot">
    <CKE5UIPart Name="toolbar" Class="mb-4" />

    <CKE5Editable RootName="header" Value="<p>Header content</p>" />
    <CKE5Editable RootName="content" Value="<p>Main content</p>" />
    <CKE5Editable RootName="footer" Value="<p>Footer content</p>" />
</CKE5Editor>
```

> [!NOTE]
> `EditorId` is passed down automatically to `CKE5Editable` and `CKE5UIPart` components via cascading parameters if they are placed inside the `<CKE5Editor>`. If placed outside, you must manually set `EditorId` on each of them to link them to the specific editor. Otherwise, all editables will bind to the first editor instance found in the DOM.

## Advanced configuration ⚙️

### Blazor Data Binding 🔄

Use native Blazor binding and callbacks for full client ⇄ server synchronization.

![CKEditor 5 Live Sync demo](docs/live-sync.gif)

#### Two way binding using `@bind-Value` ⛓️

Bind editor content to your component state.

```razor
<CKE5Editor
    @bind-Value="content"
    SaveDebounceMs="500" />

@code {
    private EditorValue content = "<p>Initial content</p>";
}
```

##### Multiroot Editables 🌳⛓️

For multiroot/decoupled layouts, bind each root independently:

```razor
<CKE5Editable RootName="header" @bind-Value="header" />
<CKE5Editable RootName="content" @bind-Value="content" />

@code {
    private string header = "<p>Header</p>";
    private string content = "<p>Main</p>";
}
```

> [!NOTE]
> `EditorId` is passed down automatically to `CKE5Editable` and `CKE5UIPart` components via cascading parameters if they are placed inside the `<CKE5Editor>`. If placed outside, you must manually set `EditorId` on each of them to link them to the specific editor. Otherwise, all editables will bind to the first editor instance found in the DOM.

#### Bidirectional Communication using Events 🔄

##### Editor → .NET: Content Change Event 📤

Observe content updates without replacing your binding logic:

```razor
<CKE5Editor
    @bind-Value="value"
    OnChange="OnEditorChange" />

@code {
    private EditorValue value = "<p>Hello</p>";

    private void OnEditorChange(CKE5EditorChangeEventArgs args)
    {
        Console.WriteLine(args.Value.Data["main"]);
    }
}
```

##### .NET → Editor: Set Content 📥

Update bound value from C# and editor content is pushed automatically:

```razor
<button @onclick="LoadTemplate">Load template</button>
<CKE5Editor @bind-Value="value" />

@code {
    private EditorValue value = "<p>Initial</p>";

    private void LoadTemplate()
        => value = "<h2>Work Report</h2><p>This is a template loaded from .NET.</p>";
}
```

### Editor Ready Event ✅

An event is fired when the editor has finished initializing and is fully ready.
This can be useful for triggering UI updates, focusing related components, or
performing any logic that must wait until the editor is available.

```razor
@using Microsoft.JSInterop

<CKE5Editor OnReady="OnReady" />

@code {
    private void OnReady(IJSObjectReference editor)
    {
        Console.WriteLine("Editor is ready");
    }
}
```

### Focus Tracking 👁️

You can track editor focus state using `OnFocus` and `OnBlur` events. This is useful for UI adjustments or validation logic based on whether the editor is active.

```razor
<CKE5Editor
    OnFocus="() => isFocused = true"
    OnBlur="() => isFocused = false" />

@code {
    private bool isFocused;
}
```

### Image Upload 🖼️

The editor supports image uploads triggered by drag-and-drop, clipboard paste, or the toolbar image button.

Behavior depends on whether the `OnImageUpload` callback is set:

- **With `OnImageUpload`** - the file is encoded as Base64 and passed to your .NET handler. Your handler stores it wherever you like (disk, cloud, database) and returns the public URL to embed in the document.
- **Without `OnImageUpload`** - the editor falls back to embedding the image as a Base64 `data:` URI directly in the content. This is fine for quick prototyping but not recommended for production because it significantly inflates document size.

```razor
@using CKEditor.Blazor.Components
@using CKEditor.Blazor.Model
@using CKEditor.Blazor.Model.Events

<CKE5Editor
    @bind-Value="content"
    OnImageUpload="HandleImageUpload" />

@code {
    private EditorValue content = "<p>Drop an image here.</p>";

    private async Task<string?> HandleImageUpload(CKE5ImageUploadEventArgs args)
    {
        // args.FileName  — original file name, e.g. "photo.jpg"
        // args.MimeType  — MIME type, e.g. "image/jpeg"
        // args.Payload   — Base64-encoded file content

        var bytes = Convert.FromBase64String(args.Payload);

        // Save to your storage and return the public URL:
        var url = await MyStorageService.SaveAsync(args.FileName, args.MimeType, bytes);

        return url; // CKEditor 5 embeds this URL in the document
    }
}
```

> [!NOTE]
> The `OnImageUpload` callback must be set on a server-interactive component (`@rendermode InteractiveServer` or `InteractiveWebAssembly`). It will not be invoked in static rendering mode.

### Watchdog 🐶

By default, the editor is wrapped in a watchdog that automatically tries to recover from crashes by reinitializing the editor instance. This ensures a more resilient user experience, especially in cases where custom plugins or configurations might cause instability.

#### Disabling the watchdog 🚫

In some scenarios, such as when using a highly customized editor setup or when you want to handle errors manually, you might want to disable the watchdog. You can do this by setting the `Watchdog` parameter to `false` on the `CKE5Editor` component:

```razor
<CKE5Editor Watchdog="false" />
```

### Splitting Assets: Global Import Map with Per-page Styles 🗺️

The typical setup places `<CKE5Assets Distribution="..." />` in the shared `<head>` of your layout. This works perfectly when most routes use the editor.

If your app only uses the editor on specific pages, loading the import map globally is fine, but stylesheets add unnecessary overhead on pages without the editor. You can solve this by splitting the assets.

Place `<CKE5Importmap Distribution="..." />` in your shared layout `<head>`, as the import map must appear before any scripts that use it. Then, place `<CKE5Assets Distribution="..." EmitImportMap="false" />` only on pages that actually render the editor. This ensures stylesheets and preload hints are loaded only when needed.

#### Self-hosted variant 🏠

**`App.razor` (shared layout `<head>`):**

```razor
@using CKEditor.Blazor.Components.Assets
@using CKEditor.Blazor.Model.License

<HeadContent>
    @* Declared once globally. No stylesheets, no preload hints. *@
    <CKE5Importmap Distribution="DistributionChannel.SH" />
</HeadContent>
```

**Page that uses the editor:**

```razor
@using CKEditor.Blazor.Components
@using CKEditor.Blazor.Components.Assets
@using CKEditor.Blazor.Model.License

@* Load stylesheets only on this page. *@
<CKE5Assets Distribution="DistributionChannel.SH" EmitImportMap="false" />

<CKE5Editor Value="@("<p>Hello!</p>")" />
```

#### CDN variant 📡

**`App.razor` (shared layout `<head>`):**

```razor
@using CKEditor.Blazor.Components.Assets
@using CKEditor.Blazor.Model.License

<HeadContent>
    @* Declared once globally. No stylesheets, no preload hints. *@
    <CKE5Importmap Distribution="DistributionChannel.Cloud" />
</HeadContent>
```

**Page that uses the editor:**

```razor
@using CKEditor.Blazor.Components
@using CKEditor.Blazor.Components.Assets
@using CKEditor.Blazor.Model.License

@* Load stylesheets only on this page. *@
<CKE5Assets Distribution="DistributionChannel.Cloud" EmitImportMap="false" />

<CKE5Editor Value="@("<p>Hello!</p>")" />
```

Both `CKE5Importmap` and `CKE5Assets` accept the same `Preset`, `Nonce`, and `CustomImportMap` parameters.

#### Disabling module preload hints ⏳

By default the per-page component still emits `<link rel="modulepreload">` hints, which tell the browser to fetch ESM chunks early. If you want to opt out of that too (e.g. to reduce `<head>` size on low-traffic pages), add `EmitModulePreload="false"`:

```razor
<CKE5Assets Distribution="DistributionChannel.Cloud" EmitImportMap="false" EmitModulePreload="false" />
@* or *@
<CKE5Assets Distribution="DistributionChannel.SH" EmitImportMap="false" EmitModulePreload="false" />
```

## Context 🤝

The **context** feature is designed to group multiple editor instances together, allowing them to share a common context. This is particularly useful in collaborative editing scenarios, where users can work together in real time. By sharing a context, editors can synchronize features such as comments, track changes, and presence indicators across different editor instances. This enables seamless collaboration and advanced workflows in your Phoenix application.

### Basic usage 🔧

![CKEditor 5 Context in Blazor application](docs/context.png)

```razor
<CKE5Context Id="shared-context">
    @* ContextId is inferred automatically for nested editors *@
    <CKE5Editor Value="<p>Editor 1 content</p>" />
    <CKE5Editor Value="<p>Editor 2 content</p>" />
</CKE5Context>

@* Editors outside the context can reference it by Id to share the same context *@
<CKE5Editor ContextId="shared-context" Value="<p>Editor 3 content</p>" />
```

### Custom context config 🌐

Pass a context config object directly using `ContextPreset`:

```razor
@using CKEditor.Blazor.Model

<CKE5Context
    ContextPreset="@(new ContextConfig
    {
        Plugins = new List<string> { "Essentials", "Paragraph" },
        Config = new Dictionary<string, object>
        {
            ["language"] = "pl"
        }
    })">
    <CKE5Editor Value="@("<p>Shared context</p>")" />
</CKE5Context>
```

### Context config DSL 🛠️

`ContextConfig` exposes the same fluent builder API as `PresetConfig`, so you can compose context configuration without working directly with raw collections:

```csharp
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Services;

builder.Services.AddCKEditor(options => options
    .ExtendDefaultContext(context => context
        .AddPlugins(Plugin.Import("MyCustomPlugin", "./my-custom-plugin.js")))

    .AddContext("shared", context => context
        .WithPlugins("Essentials", "Paragraph", "Bold")
        .WithLanguage("pl")
        .WithConfigEntry("toolbar", new[] { "bold" })));
```

You can also build a context inline in Razor using the same API:

```razor
@using CKEditor.Blazor.Model

<CKE5Context
    Id="shared-context"
    ContextPreset="@(new ContextConfig()
        .WithPlugins("Essentials", "Paragraph")
        .WithLanguage("pl"))">
    <CKE5Editor ContextId="shared-context" Value="@("<p>Shared context</p>")" />
</CKE5Context>
```

## Custom plugins 🧩

![Custom plugin demo](docs/custom-highlight-plugin.png)

There are two ways to register a custom plugin, depending on whether you have a JavaScript bundle in your app.

### Import from a JS module 📦

If you don't have a custom JavaScript bundle, point the editor directly at your plugin file using `Plugin.Import` in Blazor. No extra JavaScript setup is needed — the editor will load the module on demand.

```csharp
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Services;

builder.Services.AddCKEditor(options => options
    .ExtendDefaultPreset(preset => preset
        .AddPlugins(Plugin.Import("MyCustomPlugin", "./my-custom-plugin.js"))));
```

The module at `./my-custom-plugin.js` must export the plugin class as its default export:

```ts
// my-custom-plugin.js
import { Plugin } from 'ckeditor5';

export default class MyCustomPlugin extends Plugin {
  static get pluginName() {
    return 'MyCustomPlugin';
  }

  init() {
    console.log('MyCustomPlugin initialized');
  }
}
```

### Register in a JS bundle 🗂️

If your app already has a JavaScript bundle that runs before the editor, you can register plugins there using `CustomEditorPluginsRegistry`. The plugin must be registered **before** the editor initializes.

```ts
import { CustomEditorPluginsRegistry as Registry } from 'ckeditor5-blazor';

const unregister = Registry.the.register('MyCustomPlugin', async () => {
  const { Plugin } = await import('ckeditor5');

  return class extends Plugin {
    static get pluginName() {
      return 'MyCustomPlugin';
    }

    init() {
      console.log('MyCustomPlugin initialized');
    }
  };
});
```

Then reference the plugin by name in your Blazor config:

```csharp
using CKEditor.Blazor.Services;

builder.Services.AddCKEditor(options => options
    .ExtendDefaultPreset(preset => preset
        .AddPlugins("MyCustomPlugin")));
```

## Editors and Contexts registry 👀

The package provides two registries: `EditorsRegistry` and `ContextsRegistry`. They allow you to watch for changes in registered editors and contexts, get instances directly, or execute logic when a specific editor or context appears.

- **`watch(callback)`** — react whenever registry state changes.

    ```javascript
    import { EditorsRegistry } from 'ckeditor5-blazor';

    const unregisterWatcher = EditorsRegistry.the.watch((editors) => {
      console.log('Registered editors changed:', editors);
    });

    // Later, you can unregister the watcher
    unregisterWatcher();
    ```

- **`waitFor(id)`** — get the instance directly. If it is already registered, the promise resolves immediately.

    ```javascript
    import { EditorsRegistry } from 'ckeditor5-blazor';

    EditorsRegistry.the.waitFor('editor1').then((editor) => {
      console.log('Editor "editor1" is registered:', editor);
    });

    // ... init editor somewhere later
    ```

- **`execute(id, callback)`** — run logic immediately if the instance already exists, or later when it appears.

    ```javascript
    import { EditorsRegistry } from 'ckeditor5-blazor';

    EditorsRegistry.the.execute('editor1', (editor) => {
      console.log('Current data:', editor.getData());
    });
    ```

- The same methods are available on `ContextsRegistry` for shared contexts:

    ```javascript
    import { ContextsRegistry } from 'ckeditor5-blazor';

    ContextsRegistry.the.waitFor('shared-context').then((watchdog) => {
      console.log('Context is ready:', watchdog.context);
    });

    ContextsRegistry.the.execute('shared-context', (watchdog) => {
      console.log('Context state:', watchdog.state);
    });
    ```

## Development ⚙️

To start the development environment, run:

```bash
pnpm run dev
```

The playground app will be available at [http://localhost:5173](http://localhost:5173).

### Running Tests 🧪

Run JavaScript package tests:

```bash
pnpm run npm_package:test
```

Run .NET tests with coverage report:

```bash
pnpm run dotnet:test
```

#### Running E2E tests 🧪

Make sure the development environment is running (`pnpm run dev`), then execute:

```bash
pnpm run dotnet:e2e:headed
```

## Psst... 👀

If you're looking for similar stuff, check these out:

- [ckeditor5-phoenix](https://github.com/Mati365/ckeditor5-phoenix)
  Seamless CKEditor 5 integration for Phoenix Framework. Plug & play support for LiveView forms with dynamic content, localization, and custom builds.

- [ckeditor5-rails](https://github.com/Mati365/ckeditor5-rails)
  Smooth CKEditor 5 integration for Ruby on Rails. Works with standard forms, Turbo, and Hotwire. Easy setup, custom builds, and localization support.

- [ckeditor5-symfony](https://github.com/Mati365/ckeditor5-symfony)
  Native CKEditor 5 integration for Symfony. Works with Symfony 6.x+, standard forms and Twig. Supports custom builds, multiple editor configurations, asset management, and localization. Designed to be simple, predictable, and framework-native.

- [ckeditor5-livewire](https://github.com/Mati365/ckeditor5-livewire)
  CKEditor 5 integration for Laravel Livewire. Real-time syncing, custom builds, localization, and easy setup.

## Trademarks 📜

CKEditor® is a trademark of [CKSource Holding sp. z o.o.](https://cksource.com/) All rights reserved. For more information about the license of CKEditor® please visit [CKEditor's licensing page](https://ckeditor.com/legal/ckeditor-oss-license/).

This package is not owned by CKSource and does not use the CKEditor® trademark for commercial purposes. It should not be associated with or considered an official CKSource product.

## License 📜

This project is licensed under the terms of the [MIT LICENSE](LICENSE).

This project injects CKEditor 5 which is licensed under the terms of [GNU General Public License Version 2 or later](https://www.gnu.org/licenses/old-licenses/gpl-2.0.html). For more information about CKEditor 5 licensing, please see their [official documentation](https://ckeditor.com/legal/ckeditor-oss-license/).
