using System.IO.Compression;
using System.Runtime.InteropServices;
using Octokit;
using VectraLauncher.Models;
using VectraLauncher.Utilities;
using FileMode = System.IO.FileMode;

namespace VectraLauncher.Commands;

internal static class InstallCommand
{
    public static async Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error: Version number required");
            Console.WriteLine("Usage: vecc install <version>");
            return 1;
        }

        var version = args[0];

        if (!SemanticVersion.TryParse(version, out _))
        {
            Console.WriteLine($"Error: Invalid version format '{version}'. Expected format: x.y.z");
            return 1;
        }

        if (VersionManager.VersionExists(version))
        {
            Console.WriteLine($"Version {version} is already installed");
            return 0;
        }

        try
        {
            await DownloadAndInstallAsync(version);
            var config = VersionManager.LoadConfiguration();
            var versionPath = $"versions/{version}";
            config.InstalledVersions.Add(new InstalledVersion
            {
                Version = version,
                Path = versionPath
            });

            var previousActive = config.ActiveVersion;
            VersionManager.RecalculateLatestVersion(config);
            VersionManager.SaveConfiguration(config);

            Console.WriteLine($"Successfully installed version {version}");

            if (previousActive == "latest")
            {
                Console.WriteLine($"Active version (latest) now resolves to: {config.ResolvedActiveVersion}");
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Installation failed - {ex.Message}");
            return 1;
        }
    }

    private static async Task DownloadAndInstallAsync(string version)
    {
        var client = new GitHubClient(new ProductHeaderValue("vecc"));
        var releases = await client.Repository.Release.GetAll("DuncanMcPherson", "vectra");
        var release = releases.FirstOrDefault(r =>
            r.TagName.TrimStart('v') == version);
        if (release is null)
            throw new Exception($"Version {version} not found");

        var rid = GetCurrentRid();
        var assetName = $"vectra-{rid}.zip";
        var asset = release.Assets.FirstOrDefault(a => a.Name == assetName);
        if (asset is null)
            throw new Exception($"No release asset found for {rid}");

        await DownloadAssetAsync(asset.BrowserDownloadUrl, version);
    }

    private static async Task DownloadAssetAsync(string url, string version)
    {
        var versionDir = PathManager.GetVersionDirectory(version);
        Directory.CreateDirectory(versionDir);

        var tempZipPath = Path.Combine(Path.GetTempPath(), $"vectra-{version}.zip");

        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? 0;
        var downloadedBytes = 0L;
        
        Console.WriteLine($"Downloading version {version}...");
        await using var contentStream = await response.Content.ReadAsStreamAsync();
        await using (var fileStream =
                     new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
        {
            var buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                downloadedBytes += bytesRead;
                if (totalBytes > 0)
                {
                    var percentage = (int)((downloadedBytes * 100) / totalBytes);
                    Console.Write($"\rProgress: {percentage}% ({downloadedBytes / 1024}KB / {totalBytes / 1024}KB)");
                }
            }
        }
        
        Console.WriteLine("\nExtracting...");
        var extractedPath = await ExtractToTempAndDelete(tempZipPath, version);

        MoveDirectory(extractedPath, versionDir);
        Directory.Delete(extractedPath, true);
        
        var compilerPath = PathManager.GetCompilerPath(version);
        if (!File.Exists(compilerPath)) 
            throw new Exception($"Installation verification failed: {compilerPath} not found");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var chmod = System.Diagnostics.Process.Start("chmod", $"+x {compilerPath}");
            await chmod.WaitForExitAsync();
        }
    }

    private static void MoveDirectory(string source, string destination)
    {
        var rid = GetCurrentRid();
        var nestedDir = Path.Combine(source, rid);

        if (!Directory.Exists(nestedDir))
            throw new Exception($"Expected directory '{rid}' not found in release archive");
        foreach (var file in Directory.GetFiles(nestedDir, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(nestedDir, file);
            var destFile = Path.Combine(destination, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
            File.Move(file, destFile, overwrite: true);
        }
    }

    private static async Task<string> ExtractToTempAndDelete(string zipPath, string version)
    {
        var tempExtractPath = Path.Combine(Path.GetTempPath(), $"vectra-{version}-extract");
        await ZipFile.ExtractToDirectoryAsync(zipPath, tempExtractPath);
        File.Delete(zipPath);
        return tempExtractPath;
    }

    private static string GetCurrentRid()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "win-x64";
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "linux-x64";
        
        throw new PlatformNotSupportedException("Only Windows and Linux x64 are supported");
    }
}