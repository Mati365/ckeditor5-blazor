# ckeditor5-blazor

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-green.svg?style=flat-square)](http://makeapullrequest.com)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/mati365/ckeditor5-blazor?style=flat-square)
[![GitHub issues](https://img.shields.io/github/issues/mati365/ckeditor5-blazor?style=flat-square)](https://github.com/Mati365/ckeditor5-blazor/issues)
![NPM Version](https://img.shields.io/npm/v/ckeditor5-blazor?style=flat-square)

CKEditor 5 for Blazor — a lightweight WYSIWYG editor integration for ASP.NET Core Blazor Server and WebAssembly. It works with Razor components and .NET forms. Easy to set up, it supports self-hosted assets, CDN loading, multiple editor types, shared contexts, localization, and custom plugins.

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
  - [Configuration ⚙️](#configuration-️)
    - [Override default preset configuration 🧑‍💻](#override-default-preset-configuration-)
    - [Define your configuration directly in the view 💻](#define-your-configuration-directly-in-the-view-)
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
    - [Watchdog 🐶](#watchdog-)
      - [How it works ⚙️](#how-it-works-️)
      - [Disabling the watchdog 🚫](#disabling-the-watchdog-)
  - [Context 🤝](#context-)
    - [Basic usage 🔧](#basic-usage--1)
    - [Custom context config 🌐](#custom-context-config-)
  - [Custom plugins 🧩](#custom-plugins-)
  - [Editors and Contexts registry 👀](#editors-and-contexts-registry-)
  - [Development ⚙️](#development-️)
    - [Running Tests 🧪](#running-tests-)
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
   dotnet add package CKEditor5.Blazor
   ```

2. **Register CKEditor services** in `Program.cs`:

   ```csharp
   using CKEditor.Blazor.Services;

   builder.Services.AddCKEditor();
   ```

3. **(Optional) Override MSBuild asset options** in your `.csproj`:

   ```xml
   <PropertyGroup>
     <CKEditorVersion>47.3.0</CKEditorVersion>
     <CKEditorIncludePremiumAssets>false</CKEditorIncludePremiumAssets>
     <CKBoxVersion>2.8.0</CKBoxVersion>
     <CKBoxIncludeAssets>true</CKBoxIncludeAssets>
     <CKEditorAssetsOutputPath>$(MSBuildProjectDirectory)/wwwroot</CKEditorAssetsOutputPath>
   </PropertyGroup>
   ```

4. **Build your project** to download and prepare assets:

   ```bash
   dotnet build
   ```

5. **Add self-hosted assets component** in `<head>` (e.g. `App.razor`):

   ```razor
   @using CKEditor.Blazor.Components.Assets

   <HeadContent>
       <CKE5SelfHosted />
   </HeadContent>
   ```

### 📡 CDN Distribution

Load CKEditor 5 from CKSource CDN using import maps. This method avoids local asset downloads and is good for quick setup.

**Complete setup:**

1. **Add NuGet dependency:**

   ```bash
   dotnet add package CKEditor5.Blazor
   ```

2. **Register CKEditor with cloud preset** in `Program.cs`:

   ```csharp
   using CKEditor.Blazor.Model.Cloud;
   using CKEditor.Blazor.Services;

   builder.Services.AddCKEditor(options =>
   {
       options.DefaultLicenseKey = "your-license-key-here";

       options.Presets["default"] = ConfigManager.CreateDefaultPreset(
           cloudConfig: new CloudConfig
           {
               EditorVersion = "47.3.0",
               Premium = false
           });
   });
   ```

3. **Add cloud assets component** in `<head>`:

   ```razor
   @using CKEditor.Blazor.Components.Assets

   <HeadContent>
       <CKE5Cloud />
   </HeadContent>
   ```

4. **Use editor components** anywhere in your Razor UI:

   ```razor
   <CKE5Editor Value="<p>Hello world!</p>" />
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
    EditorType="classic"
    Value="<p>Initial content</p>"
    EditableHeight="300"
    @bind-Value="content" />

@code {
    private EditorValue content = "<p>Initial content</p>";
}
```

## Configuration ⚙️

You can configure editor presets in `AddCKEditor(...)`. The default preset is `default`. Presets are reusable configuration objects that can be applied to any editor instance. You can also define custom presets and override the default one.

### Override default preset configuration 🧑‍💻

You can pass initial content and merge additional configuration. In scenario below, the `MergeConfig` will extend the `default` preset configuration to make the menu bar visible. It's only shallow merge, so nested arrays will be replaced, not merged.

```razor
<CKE5Editor
    Value="<p>This is the initial content of the editor.</p>"
    MergeConfig="@(new Dictionary<string, object>
    {
        [\"menuBar\"] = new Dictionary<string, object>
        {
            [\"isVisible\"] = true
        }
    })" />
```

### Define your configuration directly in the view 💻

Override the default configuration with custom plugins and toolbar items. In this example, the editor will only have `Essentials`, `Paragraph`, `Bold`, `Italic`, `Link`, and `Undo` plugins, and the toolbar will contain only bold, italic, link, undo, and redo buttons. The editor locale is set to Polish (`pl`), and a custom translation for the "Bold" label is provided.

```razor
<CKE5Editor
    Language="pl"
    Config="@(new Dictionary<string, object>
    {
        [\"plugins\"] = new[] { \"Essentials\", \"Paragraph\", \"Bold\", \"Italic\", \"Link\", \"Undo\" },
        [\"toolbar\"] = new Dictionary<string, object>
        {
            [\"items\"] = new[] { \"bold\", \"italic\", \"link\", \"|\", \"undo\", \"redo\" }
        }
    })" />
```

### Define reusable configuration presets 🧩

In order to override the default preset or add custom presets, publish the configuration file:

```csharp
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Services;

builder.Services.AddCKEditor(options =>
{
    options.Presets["minimal"] = new PresetConfig
    {
        EditorType = EditorType.Classic,
        Config = new Dictionary<string, object>
        {
            ["plugins"] = new[] { "Essentials", "Paragraph", "Bold", "Italic", "Undo" },
            ["toolbar"] = new Dictionary<string, object>
            {
                ["items"] = new[] { "bold", "italic", "|", "undo", "redo" }
            }
        }
    };
});
```

Use it in Razor:

```razor
<CKE5Editor Preset="minimal" Value="<p>Simple editor</p>" />
```

### Dynamic presets 🎯

You can also create dynamic presets that can be modified at runtime. This is useful if you want to change the editor configuration based on user input or other conditions.

```razor
@using CKEditor.Blazor.Model
@using CKEditor.Blazor.Services

<CKE5Editor Preset="@dynamicPreset" Value="<p>Runtime preset</p>" />

@code {
    private readonly PresetConfig dynamicPreset = ConfigManager.CreateDefaultPreset() with
    {
        Config = new Dictionary<string, object>
        {
            ["toolbar"] = new Dictionary<string, object>
            {
                ["items"] = new[] { "bold", "italic", "link", "|", "undo", "redo" }
            }
        }
    };
}
```

### Element references using `$element` 🎯

Similarly to translation references, configuration objects may reference DOM elements by CSS selector. Use `PresetElementSelector` in C# (which serializes to `{ "$element": "selector" }`) anywhere in your editor configuration where CKEditor expects an `HTMLElement`, and the package will resolve it to the matching DOM element during initialization.

This is useful, for example, when pointing a plugin to an external container element:

```csharp
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Services;

builder.Services.AddCKEditor(options =>
{
    options.Presets["default"] = ConfigManager.CreateDefaultPreset() with
    {
        Config = new Dictionary<string, object>
        {
            ["myPlugin"] = new Dictionary<string, object>
            {
                ["container"] = new PresetElementSelector("#my-container")
            }
        }
    };
});
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
   builder.Services.AddCKEditor(options =>
   {
       options.DefaultLicenseKey = "your-license-key-here";
   });
   ```

If you use CKEditor 5 under GPL, use `GPL` as your key value.

## Localization 🌍

Support multiple languages in the editor UI and content. Configure translation loading, custom dictionaries, and reuse translation keys or DOM element references across your configuration.

### Translation Loading 🌐

For self-hosted setups, translation assets are handled by your bundler automatically. For cloud setups, translations are loaded through the configured CDN bundle. In both cases, set the UI language per editor or context:

```razor
<CKE5Editor
    Language="pl"
    Value="<p>Treść z polskim UI</p>" />
```

### Global Translation Config 🛠️

Set default language and translated labels in your preset configuration:

```csharp
builder.Services.AddCKEditor(options =>
{
    options.Presets["default"] = ConfigManager.CreateDefaultPreset() with
    {
        Config = new Dictionary<string, object>
        {
            ["language"] = "pl"
        },
        Translations = new Dictionary<string, string>
        {
            ["Bold"] = "Pogrubienie"
        }
    };
});
```

### Custom translations 🌐

You can override translations per editor instance via `CustomTranslations`:

```razor
<CKE5Editor
    Value="<p>Custom labels</p>"
    CustomTranslations="@(new Dictionary<string, string>
    {
        [\"Bold\"] = \"Pogrubienie (custom)\"
    })" />
```

#### Translation references using `$translation` ✨

In addition to supplying full translation maps, configuration objects may contain reference helpers that point to existing translation keys. This is particularly handy when you want to reuse an existing label or avoid repeating the same string in multiple places. Use `PresetTranslationReference` in C# (which serializes to `{ "$translation": "key" }`) in any part of your editor or context configuration, and the package will automatically replace it with the correct localized string during initialization.

```csharp
using CKEditor.Blazor.Model;
using CKEditor.Blazor.Services;

builder.Services.AddCKEditor(options =>
{
    options.Presets["default"] = ConfigManager.CreateDefaultPreset() with
    {
        Config = new Dictionary<string, object>
        {
            ["toolbar"] = new Dictionary<string, object>
            {
                ["items"] = new object[]
                {
                    new Dictionary<string, object>
                    {
                        ["label"] = new PresetTranslationReference("Bold"),
                        ["items"] = new[] { "bold" }
                    }
                }
            }
        },
        Translations = new Dictionary<string, string>
        {
            ["Bold"] = "Pogrubienie"
        }
    };
});
```

When the editor or context is created, the helper will be resolved against the loaded translations (including any custom translations you provided). If the key is not found, a warning is printed and `null` will be used instead.

## Editor Types 🖊️

CKEditor 5 for Blazor supports four distinct editor types, each designed for specific use cases. Choose the one that best fits your application's layout and functionality requirements.

### Classic editor 📝

Traditional WYSIWYG editor with a fixed toolbar above the editing area. Best for standard content editing scenarios like blog posts, articles, or forms.

![CKEditor 5 Classic Editor in Blazor application](docs/classic.png)

```razor
<CKE5Editor
    EditorType="classic"
    Value="<p>This is the initial content of the editor.</p>"
    EditableHeight="300" />
```

### Inline editor 📝

Minimalist editor that appears directly within content when clicked. Ideal for in-place editing scenarios where the editing interface should be invisible until needed.

![CKEditor 5 Inline Editor in Blazor application](docs/inline-editor.png)

```razor
<CKE5Editor
    EditorType="inline"
    Value="<p>Inline editor content</p>"
    Class="border border-gray-300" />
```

**Note:** Inline editors don't work with `<textarea>` elements and may not be suitable for traditional form scenarios.

### Decoupled editor 🌐

Flexible editor where toolbar and editing area are completely separated. Provides maximum layout control for custom interfaces and complex applications.

![CKEditor 5 Decoupled Editor in Blazor application](docs/decoupled-editor.png)

```razor
<CKE5Editor
    Id="decoupled-editor"
    EditorType="decoupled"
    Value="<p>Editor instance content</p>">
    <CKE5UIPart Name="toolbar" EditorId="decoupled-editor" Class="mb-4" />

    <CKE5Editable
        EditorId="decoupled-editor"
        RootName="main"
        Value="<p>This is the initial content of the decoupled editor editable.</p>"
        InnerClass="p-4" />
</CKE5Editor>
```

### Multiroot editor 🌳

Advanced editor supporting multiple separate editing areas (roots) with a shared toolbar. Perfect for complex documents with multiple editable sections like headers, sidebars, and main content.

![CKEditor 5 Multiroot Editor in Blazor application](docs/multiroot-editor.png)

```razor
<CKE5Editor
    Id="multiroot-editor"
    EditorType="multiroot"
    Value="@(new Dictionary<string, string>
    {
        [\"header\"] = \"<p>Header content</p>\",
        [\"content\"] = \"<p>Main content</p>\",
        [\"footer\"] = \"<p>Footer content</p>\"
    })" />

<CKE5UIPart Name="toolbar" EditorId="multiroot-editor" Class="mb-4" />

<CKE5Editable EditorId="multiroot-editor" RootName="header" Value="<p>Header content</p>" />
<CKE5Editable EditorId="multiroot-editor" RootName="content" Value="<p>Main content</p>" />
<CKE5Editable EditorId="multiroot-editor" RootName="footer" Value="<p>Footer content</p>" />
```

## Advanced configuration ⚙️

### Blazor Data Binding 🔄

Use native Blazor binding and callbacks for full client ⇄ server synchronization.

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
<CKE5Editable EditorId="multiroot-editor" RootName="header" @bind-Value="header" />
<CKE5Editable EditorId="multiroot-editor" RootName="content" @bind-Value="content" />

@code {
    private string header = "<p>Header</p>";
    private string content = "<p>Main</p>";
}
```

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

### Watchdog 🐶

#### How it works ⚙️

`CKE5Editor` uses watchdog by default (`Watchdog="true"`) to recreate editor instances after runtime crashes.

#### Disabling the watchdog 🚫

By default, the editor is wrapped in a watchdog that automatically tries to recover from crashes by reinitializing the editor instance. This ensures a more resilient user experience, especially in cases where custom plugins or configurations might cause instability.

```razor
<CKE5Editor Watchdog="false" />
```

## Context 🤝

The **context** feature is designed to group multiple editor instances together, allowing them to share a common context. This is particularly useful in collaborative editing scenarios, where users can work together in real time. By sharing a context, editors can synchronize features such as comments, track changes, and presence indicators across different editor instances. This enables seamless collaboration and advanced workflows in your Phoenix application.

### Basic usage 🔧

![CKEditor 5 Context in Blazor application](docs/context.png)

```razor
<CKE5Context Id="shared-context">
    <CKE5Editor ContextId="shared-context" Value="<p>Editor 1 content</p>" />
    <CKE5Editor ContextId="shared-context" Value="<p>Editor 2 content</p>" />
</CKE5Context>
```

### Custom context config 🌐

Pass a context config object directly using `ContextPreset`:

```razor
@using CKEditor.Blazor.Model

<CKE5Context
    Id="shared-context"
    ContextPreset="@(new ContextConfig
    {
        Plugins = new List<string> { \"Essentials\", \"Paragraph\" },
        Config = new Dictionary<string, object>
        {
            [\"language\"] = \"pl\"
        }
    })">
    <CKE5Editor ContextId="shared-context" Value="<p>Współdzielony context</p>" />
</CKE5Context>
```

## Custom plugins 🧩

Register custom plugins in JavaScript/TypeScript using `CustomEditorPluginsRegistry`:

![Custom plugin demo](docs/custom-highlight-plugin.png)

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

Then reference plugin name in editor config (preset or `Config`):

```csharp
using CKEditor.Blazor.Services;

builder.Services.AddCKEditor(options =>
{
    options.Presets["default"] = ConfigManager.CreateDefaultPreset() with
    {
        Config = new Dictionary<string, object>
        {
            ["plugins"] = new[] { "Essentials", "Paragraph", "MyCustomPlugin" }
        }
    };
});
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

The playground app will be available at [http://localhost:8000](http://localhost:8000).

### Running Tests 🧪

Run JavaScript package tests:

```bash
pnpm run npm_package:test
```

Run .NET tests with coverage report:

```bash
pnpm run dotnet:test
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
