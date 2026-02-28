# ckeditor5-blazor

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-green.svg?style=flat-square)](http://makeapullrequest.com)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/mati365/ckeditor5-blazor?style=flat-square)
[![GitHub issues](https://img.shields.io/github/issues/mati365/ckeditor5-blazor?style=flat-square)](https://github.com/Mati365/ckeditor5-blazor/issues)
![NPM Version](https://img.shields.io/npm/v/ckeditor5-blazor?style=flat-square)

The fastest way to add CKEditor 5 to your Blazor app. Zero-config installation, native C# data binding, and no manual JS Interop.

> [!IMPORTANT]
> This integration is unofficial and not maintained by CKSource. For official CKEditor 5 documentation, visit [ckeditor.com](https://ckeditor.com/docs/ckeditor5/latest/). If you encounter any issues in editor, please report them on the [GitHub repository](https://github.com/ckeditor/ckeditor5/issues).

<!-- markdownlint-disable MD033 -->
<p align="center">
  <img src="docs/intro-classic-editor.png" alt="CKEditor 5 Classic Editor in .NET / Blazor application">
</p>

## Table of Contents

- [ckeditor5-blazor](#ckeditor5-blazor)
  - [Table of Contents](#table-of-contents)
  - [Under construction 🚧](#under-construction-)
  - [Installation 🚀](#installation-)
    - [Self hosted 🏠](#self-hosted-)
    - [Cloud hosted ☁️](#cloud-hosted-️)
  - [Basic Usage 🏁](#basic-usage-)
  - [Editors and Contexts registry 👀](#editors-and-contexts-registry-)
  - [Development ⚙️](#development-️)
  - [Psst... 👀](#psst-)
  - [Trademarks 📜](#trademarks-)
  - [License 📜](#license-)

## Under construction 🚧

This project is currently under active development. In the meantime the author might be watching cat videos, brewing coffee, and negotiating with npm packages - PRs and snacks are welcome. Here's a silly cat+coffee picture for motivation:

![Cat and Coffee](https://i.makeagif.com/media/6-02-2015/ipXidH.gif)

## Installation 🚀

Add the package to your Blazor project using NuGet:

```bash
dotnet add package CKEditor5.Blazor
```

and register service in `Program.cs`:

```csharp
builder.Services.AddCKEditor5();
```

Depending on your needs, you can choose between two installation methods: `self-hosted` or `cloud-hosted`.

### Self hosted 🏠

Bundle CKEditor 5 with your application for full control over assets, versioning, and offline support. This method automatically downloads the necessary assets from NPM into your project's `wwwroot` directory during the build process using MSBuild tasks.

1. **Configure MSBuild (Optional):**

    The package comes with default configuration settings. You can override these in your project's .csproj file to control the version or enable premium features:

    ```xml
    <PropertyGroup>
      <CKEditorVersion>47.3.0</CKEditorVersion>
      <CKEditorIncludePremiumAssets>false</CKEditorIncludePremiumAssets>
      <CKEditorAssetsOutputPath>$(MSBuildProjectDirectory)/wwwroot</CKEditorAssetsOutputPath>
    </PropertyGroup>
    ```

2. **Rebuild your project:**

    It'll trigger the asset download and bundling process. The CKEditor 5 assets will be available in the specified output path (default is `wwwroot/ckeditor5`).

    ```bash
    dotnet build
    ```

3. **Add Assets to your layout:**

    In your main layout file (e.g., App.razor, _Host.cshtml, or MainLayout.razor), add the self-hosted assets component inside the <head> tag:

    ```razor
    @using CKEditor.Blazor.Components.Assets

    <HeadContent>
        <CKE5SelfHosted />
    </HeadContent>
    ```

### Cloud hosted ☁️

Load CKEditor 5 directly from CKSource's CDN. This method requires no build configuration or asset management but **requires a valid License Key** (even for trial purposes).

1. **Configure License Key:**

    Adjust your `AddCKEditor5` service registration in `Program.cs` to include your CKEditor Cloud License Key:

    ```csharp
    builder.Services.AddCKEditor5(options =>
    {
        options.DefaultLicenseKey = "your-license-key-here";
    });
    ```

2. **Add Assets to Layout:**

    In your main layout file (e.g., App.razor, _Host.cshtml, or MainLayout.razor), add the cloud-hosted assets component inside the <head> tag:

    ```razor
    @using CKEditor.Blazor.Components

    <HeadContent>
        <CKE5Cloud />
    </HeadContent>
    ```

## Basic Usage 🏁

You can now use the `<CKE5Editor>` component anywhere in your Blazor app.

```razor
@using CKEditor.Blazor.Components

<CKE5Editor EditorType="classic" Value="@("<p>Hello world!</p>")" />
```

In scenarios where you need a standalone editable root (for example in a multiroot layout,
sidebar, or custom UI part) you can use the `<CKE5Editable>` component. It behaves much like
`<CKE5Editor>` but exposes only a single root and must be attached to an existing editor via
`EditorId`.

Both components support two‑way data binding via `Value`/`@bind-Value` **and** an `OnChange`
event callback which fires every time the editor data changes. This is useful when you want
to observe edits without mutating the bound value. The callback now provides a reference to
the underlying CKEditor instance (analogous to the `OnFocus`/`OnBlur` events on `<CKE5Editor>`),
so handlers can invoke JS methods directly or inspect the editor if needed.

- The `EditorType` parameter accepts any CKEditor 5 build (e.g., `classic`, `inline`, `balloon`, `decoupled` or `multiroot`).

- The `Value` parameter allows you to set the initial content of the editor, and supports two-way binding with `@bind-Value`. Keep in mind that `Value` is [`EditorValue.cs`](/src/CKEditor.Blazor/Model/EditorValue.cs) type, which also supports multiple roots. If you use classic editor, which has only single root, you can pass string content directly. For editors with multiple roots, you need to pass a directory with root names as keys and their content as values.

- **Change notifications** – in addition to two‑way binding you may register a callback that fires every time the editor data changes without mutating your bound value. Use the `OnChange` parameter to receive both the new value and a JS object reference for the editor:

```razor
<CKE5Editor
    EditorType="classic"
    OnChange="@(args => Console.WriteLine($"Change: {args.Value}") )"
/>

<CKE5Editable
    EditorId="someId"
    OnChange="@(args => Console.WriteLine($"Editable changed: {args.Data}"))"
/>
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
