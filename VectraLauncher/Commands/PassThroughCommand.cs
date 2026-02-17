using System.Diagnostics;
using VectraLauncher.Utilities;

namespace VectraLauncher.Commands;

internal static class PassThroughCommand
{
    public static async Task<int> ExecuteAsync(string[] args)
    {
        var compilerPath = VersionManager.GetActiveCompilerPath();
        if (compilerPath is null)
        {
            Console.WriteLine("Error: No active compiler version set.");
            Console.WriteLine("Get started by running 'vecc install latest'");
            return 1;
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = compilerPath,
                Arguments = string.Join(" ", args),
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                WorkingDirectory = Directory.GetCurrentDirectory()
            }
        };

        process.Start();
        await process.WaitForExitAsync();

        await CheckAndNotifyAsync();

        return process.ExitCode;
    }

    private static async Task CheckAndNotifyAsync()
    {
        var config = VersionManager.LoadConfiguration();
        var latestAvailable = await VersionManager.CheckForUpdatesAsync();

        if (latestAvailable == null)
            return;
        var current = config.LatestInstalledVersion;
        var updateLine = $"  Update available: {current} -> {latestAvailable}  ";
        var installLine = "  Run 'vecc update' to install  ";

        var width = Math.Max(updateLine.Length, installLine.Length);
        var border = new string('-', width);

        Console.WriteLine();
        Console.WriteLine($"+{border}+");
        Console.WriteLine($"|{updateLine.PadRight(width)}|");
        Console.WriteLine($"|{installLine.PadRight(width)}|");
        Console.WriteLine($"+{border}+");
    }
}