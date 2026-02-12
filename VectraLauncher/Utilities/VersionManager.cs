using System.Text.Json;
using VectraLauncher.Models;

namespace VectraLauncher.Utilities;

internal class VersionManager
{
    // Load config from disk (or create default if doesn't exist)
    public static VectraConfiguration LoadConfiguration()
    {
        var configPath = PathManager.GetConfigPath();

        if (!File.Exists(configPath))
        {
            return new VectraConfiguration();
        }

        try
        {
            var json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<VectraConfiguration>(json) ?? new VectraConfiguration();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to load configuration: {ex.Message}");
            Console.WriteLine("Using default configuration.");
            return new VectraConfiguration();
        }
    }

    // Save config to disk
    public static void SaveConfiguration(VectraConfiguration config)
    {
        var configPath = PathManager.GetConfigPath();
        var installRoot = PathManager.GetInstallationRoot();

        Directory.CreateDirectory(installRoot);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(config, options);
        File.WriteAllText(configPath, json);
    }

    // Check if a version exists on disk
    public static bool VersionExists(string version)
    {
        var versionDir = PathManager.GetVersionDirectory(version);
        var compilerPath = PathManager.GetCompilerPath(version);

        return Directory.Exists(versionDir) && File.Exists(compilerPath);
    }

    // Get the path to the active compiler executable
    public static string? GetActiveCompilerPath()
    {
        var config = LoadConfiguration();

        if (string.IsNullOrEmpty(config.ResolvedActiveVersion))
            return null;

        var compilerPath = PathManager.GetCompilerPath(config.ResolvedActiveVersion);
        return File.Exists(compilerPath) ? compilerPath : null;
    }

    // Update latestInstalledVersion after install/uninstall
    private static void RecalculateLatestVersion(VectraConfiguration config)
    {
        config.LatestInstalledVersion = string.Empty;

        SemanticVersion? highest = null;

        foreach (var installed in config.InstalledVersions)
        {
            if (SemanticVersion.TryParse(installed.Version, out var version))
            {
                if (highest == null || version.CompareTo(highest.Value) > 0)
                {
                    highest = version;
                    config.LatestInstalledVersion = installed.Version;
                }
            }
        }

        if (config.ActiveVersion == "latest")
        {
            config.ResolvedActiveVersion = config.LatestInstalledVersion;
        }
        else
        {
            config.ResolvedActiveVersion = config.ActiveVersion;
        }
    }
}