using System.Diagnostics;

var isPremium = args.Contains("--premium");
var repoRoot = FindRepoRoot(Directory.GetCurrentDirectory());
var version = $"1.0.0-local.{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
var localFeed = Path.Combine(repoRoot, ".tmp", "local-nuget-feed");

// Grouping paths
var paths = new
{
    RclProject = Path.Combine(repoRoot, "demos", "CKEditor.Demo.RCL", "CKEditor.Demo.RCL.csproj"),
    BlazorProject = Path.Combine(repoRoot, "src", "CKEditor.Blazor", "CKEditor.Blazor.csproj"),
    WwwRoot = Path.Combine(repoRoot, "demos", "CKEditor.Demo.RCL", "wwwroot"),
    WwwRootCkeditor = Path.Combine(repoRoot, "demos", "CKEditor.Demo.RCL", "wwwroot", "ckeditor5"),
    PackageFile = Path.Combine(localFeed, $"CKEditor.Blazor.{version}.nupkg")
};

CleanDirectory(paths.WwwRootCkeditor);
Directory.CreateDirectory(localFeed);

Console.WriteLine($"📦 Packing CKEditor.Blazor ({version})...");
if (!RunCommand("dotnet", "pack", paths.BlazorProject, "-c", "Debug", "-o", localFeed, $"-p:PackageVersion={version}"))
{
    return Error("Pack failed");
}

if (!File.Exists(paths.PackageFile))
{
    return Error("ERROR: Package not found");
}

Console.WriteLine($"🔨 Building RCL with packaged version {version}...");
if (!RunCommand("dotnet", "build", paths.RclProject,
    "--no-cache",
    "--source", localFeed,
    "--source", "https://api.nuget.org/v3/index.json",
    $"-p:CKEditorBlazorPackageVersion={version}",
    "-p:CKEditorInstallAssets=true",
    $"-p:CKEditorInstallPremiumAssets={isPremium.ToString().ToLower()}"))
{
    return Error("Build failed");
}

Console.WriteLine(Directory.Exists(paths.WwwRoot)
    ? $"✅ Done! Copied {Directory.GetFiles(paths.WwwRoot, "*", SearchOption.AllDirectories).Length} files to wwwroot"
    : "⚠️  No assets copied");

return 0;

/// <summary>
/// Writes error messages to the console and returns an error code (1).
/// </summary>
static int Error(string message)
{
    Console.Error.WriteLine(message);
    return 1;
}

/// <summary>
/// Safely deletes a directory and its contents if it exists, forcing a reinstallation of assets.
/// </summary>
static void CleanDirectory(string path)
{
    if (!Directory.Exists(path))
    {
        return;
    }

    Directory.Delete(path, recursive: true);
    Console.WriteLine($"🧹 Cleaned {Path.GetFileName(path)}");
}

/// <summary>
/// Recursively searches the directory tree upwards to find the repository root based on the presence of a solution (.sln) file.
/// </summary>
static string FindRepoRoot(string startDir)
{
    var dir = new DirectoryInfo(startDir);
    while (dir != null && dir.GetFiles("*.sln").Length == 0)
    {
        dir = dir.Parent;
    }

    return dir?.FullName ?? throw new InvalidOperationException("Could not find repository root (no .sln file found).");
}

/// <summary>
/// Runs an external process and returns true if it executes successfully (exit code = 0).
/// </summary>
static bool RunCommand(string command, params string[] arguments)
{
    var psi = new ProcessStartInfo(command) { UseShellExecute = false };
    foreach (var arg in arguments)
    {
        psi.ArgumentList.Add(arg);
    }

    var process = Process.Start(psi) ?? throw new InvalidOperationException($"Failed to start {command}");
    process.WaitForExit();

    return process.ExitCode == 0;
}
