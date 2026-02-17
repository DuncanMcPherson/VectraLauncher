using VectraLauncher.Models;
using VectraLauncher.Utilities;

namespace VectraLauncher.Commands;

internal static class UseCommand
{
    public static Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error: Version required");
            Console.WriteLine("Usage: vecc use <version|latest>");
            return Task.FromResult(1);
        }
        
        var versionArg = args[0];
        var config = VersionManager.LoadConfiguration();

        if (versionArg.ToLowerInvariant() == "latest")
        {
            if (string.IsNullOrEmpty(config.LatestInstalledVersion))
            {
                Console.WriteLine("Error: No versions installed");
                return Task.FromResult(1);
            }

            config.ActiveVersion = "latest";
            VersionManager.RecalculateLatestVersion(config);
            VersionManager.SaveConfiguration(config);
            Console.WriteLine($"Switched to latest version ({config.LatestInstalledVersion})");
            return Task.FromResult(0);
        }

        if (!SemanticVersion.TryParse(versionArg, out var version))
        {
            Console.WriteLine($"Error: Invalid version format '{versionArg}'. Expected format: x.y.z");
            return Task.FromResult(1);
        }

        if (!VersionManager.VersionExists(version.ToString()))
        {
            Console.WriteLine($"Version {version} is not installed.");
            Console.WriteLine($"Install it by running `vecc install {version}`");
            return Task.FromResult(1);
        }

        config.ActiveVersion = version.ToString();
        VersionManager.RecalculateLatestVersion(config);
        VersionManager.SaveConfiguration(config);
        Console.WriteLine($"Switched to version {version}");
        return Task.FromResult(0);
    }
}