using VectraLauncher.Models;
using VectraLauncher.Utilities;

namespace VectraLauncher.Commands;

internal static class UseCommand
{
    public static async Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error: Version required");
            Console.WriteLine("Usage: vecc use <version|latest>");
            return 1;
        }
        
        var versionArg = args[0];
        var config = VersionManager.LoadConfiguration();

        if (versionArg.ToLowerInvariant() == "latest")
        {
            return await HandleUseLatest(config);
        }

        if (!SemanticVersion.TryParse(versionArg, out var version))
        {
            Console.WriteLine($"Error: Invalid version format '{versionArg}'. Expected format: x.y.z");
            return 1;
        }

        if (!VersionManager.VersionExists(version.ToString()))
        {
            Console.WriteLine($"Version {version} is not installed.");
            Console.WriteLine($"Install it by running `vecc install {version}`");
            return 1;
        }

        config.ActiveVersion = version.ToString();
        VersionManager.RecalculateLatestVersion(config);
        VersionManager.SaveConfiguration(config);
        Console.WriteLine($"Switched to version {version}");
        return 0;
    }

    private static async Task<int> HandleUseLatest(VectraConfiguration config)
    {
        var latestVersion = await VersionManager.CheckForUpdatesAsync();

        if (latestVersion != null)
        {
            Console.Write($"Warning: Latest version ({latestVersion}) is newer than currently installed version ({config.LatestInstalledVersion}). Install latest version? (Y/n): ");
            var response = Console.ReadLine();
            if (string.IsNullOrEmpty(response) || response.ToLowerInvariant() == "y")
            {
                await InstallCommand.ExecuteAsync([latestVersion]);
                config = VersionManager.LoadConfiguration();
            }
        }

        config.ActiveVersion = "latest";
        VersionManager.RecalculateLatestVersion(config);
        VersionManager.SaveConfiguration(config);
        Console.WriteLine($"Switched to latest version ({config.LatestInstalledVersion})");
        return 0;
    }
}