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

        try
        {
            Console.WriteLine($"Updating to version: {latestAvailable}");
            return await InstallCommand.ExecuteAsync([latestAvailable]);
        }
        finally
        {
            Console.WriteLine("Update complete");
        }
    }
}