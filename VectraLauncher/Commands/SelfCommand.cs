using System.Runtime.InteropServices;
using Microsoft.Win32;
using VectraLauncher.Utilities;

namespace VectraLauncher.Commands;

internal static class SelfCommand
{
    internal static async Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error: 'self' command requires a subcommand");
            Console.WriteLine("Usage: vecc self install");
            return 1;
        }

        if (args[0] != "install")
        {
            Console.WriteLine($"Error: Unknown subcommand '{args[0]}' for 'self' command");
            Console.WriteLine("Usage: vecc self install");
            return 1;
        }

        try
        {
            Console.WriteLine("Installing VectraLauncher...");
            await CopyLauncherFilesAsync();

            var pathResult = AddToPath();
            if (pathResult != 0)
                return pathResult;

            Console.Write("Would you like to install the latest compiler version? [Y/n] ");
            var response = Console.ReadLine();
            if (string.IsNullOrEmpty(response) || response.ToLowerInvariant() == "y")
            {
                var installResult = await InstallCommand.ExecuteAsync(["latest"]);
                if (installResult != 0)
                    return installResult;
            }

            Console.WriteLine("VectraLauncher installed successfully!");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Installation failed - {ex.Message}");
            return 1;
        }
    }

    private static async Task CopyLauncherFilesAsync()
    {
        var sourceDir = AppContext.BaseDirectory;
        var destDir = PathManager.GetInstallationRoot();

        Directory.CreateDirectory(destDir);

        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceDir, file);
            var destFile = Path.Combine(destDir, relative);

            Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
            await Task.Run(() => File.Copy(file, destFile, true));
        }

        Console.WriteLine("Files copied to installation directory.");
    }

    private static int AddToPath()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? AddToPathWindows()
            : AddToPathLinux();
    }

    private static int AddToPathWindows()
    {
        var installRoot = PathManager.GetInstallationRoot();

#pragma warning disable CA1416
        using var key = Registry.CurrentUser.OpenSubKey("Environment", writable: true);
#pragma warning restore CA1416
        if (key == null)
        {
            Console.WriteLine("Error: Unable to access registry to set PATH");
            return 1;
        }

#pragma warning disable CA1416
        var currentPath = key.GetValue("PATH", string.Empty).ToString() ?? string.Empty;
#pragma warning restore CA1416
        var paths = currentPath.Split(';', StringSplitOptions.RemoveEmptyEntries);
        if (paths.Contains(installRoot, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("Installation directory already in PATH.");
            return 0;
        }

        var newPath = string.IsNullOrEmpty(currentPath)
            ? installRoot
            : $"{currentPath};{installRoot}";
#pragma warning disable CA1416
        key.SetValue("PATH", newPath, RegistryValueKind.ExpandString);
#pragma warning restore CA1416

        SendMessageTimeout(
            HWND_BROADCAST,
            WM_SETTINGCHANGE,
            IntPtr.Zero,
            "Environment",
            SMTO_ABORTIFHUNG,
            5000,
            out _
        );

        Console.WriteLine($"Added {installRoot} to PATH");
        Console.WriteLine("Changes will take effect in new terminal windows.");
        return 0;
    }

    private static int AddToPathLinux()
    {
        var installRoot = PathManager.GetInstallationRoot();
        var configPath = DetectShellConfigPath(out var shellName);
        var exportLine = GetExportLine(configPath);

        if (File.Exists(configPath))
        {
            var contents = File.ReadAllText(configPath);
            if (contents.Contains(installRoot))
            {
                Console.WriteLine($"VectraLauncher is already in PATH ({configPath})");
                return 0;
            }
        }

        File.AppendAllText(configPath, $"\n# VectraLauncher\n{exportLine}\n");
        Console.WriteLine($"Added {installRoot} to PATH in {configPath}");
        Console.WriteLine($"Run 'source {configPath}' or restart your terminal for changes to take effect.");
        return 0;
    }

    private static string DetectShellConfigPath(out string shellName)
    {
        var shell = Environment.GetEnvironmentVariable("SHELL") ?? string.Empty;
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (shell.EndsWith("zsh"))
        {
            shellName = "zsh";
            return Path.Combine(home, ".zshrc");
        }

        if (shell.EndsWith("fish"))
        {
            shellName = "fish";
            return Path.Combine(home, ".config", "fish", "config.fish");
        }

        shellName = "bash";
        return Path.Combine(home, ".bashrc");
    }

    private static string GetExportLine(string configPath)
    {
        var installRoot = PathManager.GetInstallationRoot();
        return configPath.EndsWith("config.fish")
            ? $"fish_add_path {installRoot}"
            : $"export PATH=\"$PATH:{installRoot}\"";
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessageTimeout(
        IntPtr hWnd,
        uint Msg,
        IntPtr wParam,
        string lParam,
        uint fuFlags,
        uint uTimeout,
        out IntPtr lpdwResult);

    private static readonly IntPtr HWND_BROADCAST = new(0xFFFF);
    private const uint WM_SETTINGCHANGE = 0x001A;
    private const uint SMTO_ABORTIFHUNG = 0x0002;
}