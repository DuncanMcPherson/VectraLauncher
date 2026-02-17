using System.Reflection;
using VectraLauncher.Commands;

namespace VectraLauncher;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || args[0] is "--help" or "-h" or "help")
        {
            ShowHelp();
            return 0;
        }

        if (args[0] is "--version" or "-v" or "version")
        {
            ShowVersion();
            return 0;
        }

        return args[0].ToLowerInvariant() switch
        {
            "install" => await InstallCommand.ExecuteAsync(args[1..]),
            "update" => await UpdateCommand.ExecuteAsync(args[1..]),
            "list" => await ListCommand.ExecuteAsync(args[1..]),
            "use" => await UseCommand.ExecuteAsync(args[1..]),
            "uninstall" => await UninstallCommand.ExecuteAsync(args[1..]),
            "self" => await SelfCommand.ExecuteAsync(args[1..]),
            _ => await PassThroughCommand.ExecuteAsync(args)
        };
    }

    private static void ShowHelp()
    {
        var version = Assembly.GetExecutingAssembly()
            .GetName().Version;
        Console.WriteLine($"VectraLauncher v{version}");
        Console.WriteLine("A version manager for the Vectra compiler");
        Console.WriteLine();
        Console.WriteLine("Usage: vecc [command] [options]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  install <version>    Install the specified version of the Vectra compiler");
        Console.WriteLine("  update               Install the most recent version of the Vectra compiler");
        Console.WriteLine("  list                 List all available and installed versions");
        Console.WriteLine("  use <version>        Set the specified version as the active compiler");
        Console.WriteLine("  uninstall <version>  Remove the specified version from your system");
        Console.WriteLine("  self install         Add VectraLauncher to your system PATH");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --help, -h           Show this help message");
        Console.WriteLine("  --version, -v        Show version information");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  vecc install 1.2.3");
        Console.WriteLine("  vecc list");
        Console.WriteLine("  vecc update");
        Console.WriteLine("  vecc use 1.2.3");
        Console.WriteLine("  vecc uninstall 1.2.3");
        Console.WriteLine();
        Console.WriteLine("Any other commands will be passed to the active Vectra compiler.");
    }

    private static void ShowVersion()
    {
        var version = Assembly.GetExecutingAssembly()
            .GetName().Version;
        Console.WriteLine($"VectraLauncher v{version}");
    }
}