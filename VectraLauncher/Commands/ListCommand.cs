using Octokit;
using VectraLauncher.Models;
using VectraLauncher.Utilities;

namespace VectraLauncher.Commands;

internal static class ListCommand
{
    private const int DefaultVersionLimit = 10;

    public static async Task<int> ExecuteAsync(string[] args)
    {
        var showAll = args.Contains("--all");

        try
        {
            var availableVersions = await FetchAvailableVersionsAsync();

            var config = VersionManager.LoadConfiguration();
            var installedVersions = new HashSet<string>(
                config.InstalledVersions.Select(v => v.Version));

            DisplayVersionTable(availableVersions, installedVersions, config.ResolvedActiveVersion, showAll);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Failed to list versions - {ex.Message}");
            return 1;
        }
    }

    private static async Task<List<string>> FetchAvailableVersionsAsync()
    {
        var client = new GitHubClient(new ProductHeaderValue("vecc"));
        var releases = await client.Repository.Release.GetAll("DuncanMcPherson", "vectra");

        return releases.Select(r => r.TagName.TrimStart('v'))
            .Where(v => SemanticVersion.TryParse(v, out _))
            .OrderByDescending(v =>
            {
                SemanticVersion.TryParse(v, out var semver);
                return semver;
            }).ToList();
    }

    private static void DisplayVersionTable(
        List<string> availableVersions,
        HashSet<string> installedVersions,
        string activeVersion,
        bool showAll)
    {
        Console.WriteLine("Available Vectra Compiler Versions:");
        Console.WriteLine();
        Console.WriteLine("Version    Status");
        Console.WriteLine("---------  --------------------");
        var versionsToShow = showAll
            ? availableVersions
            : availableVersions.Take(DefaultVersionLimit);
        foreach (var version in versionsToShow)
        {
            var status = GetVersionStatus(version, installedVersions, activeVersion);
            Console.WriteLine($"{version,-10} {status}");
        }

        if (!showAll && availableVersions.Count > DefaultVersionLimit)
        {
            Console.WriteLine();
            Console.WriteLine($"Showing {DefaultVersionLimit} most recent versions. " +
                              "Use 'vecc list --all' to see all available versions.");
        }
    }

    private static string GetVersionStatus(string version, HashSet<string> installedVersions, string activeVersion)
    {
        if (!installedVersions.Contains(version))
            return "Not Installed";
        
        return version == activeVersion
            ? "Installed (Active)"
            : "Installed";
    }
}