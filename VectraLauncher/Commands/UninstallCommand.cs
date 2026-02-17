using VectraLauncher.Models;
using VectraLauncher.Utilities;

namespace VectraLauncher.Commands;

internal static class UninstallCommand
{
    internal static async Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length == 0 || !SemanticVersion.TryParse(args[0], out var version))
        {
            Console.WriteLine($"Error: Invalid version: '{(args.Length == 0 ? "No version provided" : args[0])}'");
            return 1;
        }

        var config = VersionManager.LoadConfiguration();
        if (!VersionManager.VersionExists(version.ToString()))
        {
            Console.WriteLine($"Error: Version '{version}' is not installed.");
            return 1;
        }
        if (config.ResolvedActiveVersion == version.ToString())
        {
            Console.WriteLine($"Error: Version '{version}' is currently active.");
            Console.WriteLine("Switch to another version first using 'vecc use <version|latest>'");
            return 1;
        }

        var versionDir = PathManager.GetVersionDirectory(version.ToString());
        Directory.Delete(versionDir, true);

        config.InstalledVersions.RemoveAll(x => x.Version == version.ToString());
        VersionManager.RecalculateLatestVersion(config);
        VersionManager.SaveConfiguration(config);

        Console.WriteLine($"Version '{version}' uninstalled successfully.");
        return 0;
    }
}