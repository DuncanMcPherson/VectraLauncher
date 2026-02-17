using VectraLauncher.Utilities;

namespace VectraLauncher.Commands;

internal static class UpdateCommand
{
    internal static async Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length > 0)
        {
            Console.WriteLine($"Error: unknown command options: '{string.Join(' ', args)}'");
            return 1;
        }

        var latestAvailable = await VersionManager.CheckForUpdatesAsync();
        if (latestAvailable is null)
        {
            Console.WriteLine("No updates available");
            return 0;
        }

        Console.WriteLine($"Updating to version: {latestAvailable}");
        var result = await InstallCommand.ExecuteAsync([latestAvailable]);
        if (result == 0)
            Console.WriteLine("Update Complete");
        return result;
    }
}