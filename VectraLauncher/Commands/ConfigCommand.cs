using VectraLauncher.Utilities;

namespace VectraLauncher.Commands;

internal static class ConfigCommand
{
    public static async Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Error: Invalid number of arguments.");
            Console.WriteLine("Usage: vecc config <setting> <value>");
            return 1;
        }

        var setting = args[0].ToLowerInvariant();
        var value = args[1];

        return setting switch
        {
            "checkdays" => await UpdateUpdateCheckFrequency(value),
            _ => await HandleUnknownSetting(setting)
        };
    }

    private static Task<int> UpdateUpdateCheckFrequency(string value)
    {
        if (!int.TryParse(value, out var days) || days <= 0)
        {
            Console.WriteLine("Error: Invalid value for checkdays. Please enter a positive integer.");
            return Task.FromResult(1);
        }

        var config = VersionManager.LoadConfiguration();
        config.AutoUpdateCheckFrequencyDays = days;
        VersionManager.SaveConfiguration(config);
        return Task.FromResult(0);
    }

    private static Task<int> HandleUnknownSetting(string setting)
    {
        Console.WriteLine($"Error: Unknown setting '{setting}'.");
        Console.WriteLine("Available settings:");
        Console.WriteLine("  CheckDays - Update how frequently to check for updates (in days)");
        return Task.FromResult(1);
    }
}