namespace VectraLauncher.Utilities;

internal static class PathManager
{
    private const string InstallationDirName = ".vectra";
    private const string VersionsDirName = "versions";

    private const string ConfigFileName = "config.json";

    // Gets installation root. ~/.vectra (Linux) or %LOCALAPPDATA%\.vectra (Windows)
    public static string GetInstallationRoot()
    {
        var baseDir = Environment.OSVersion.Platform == PlatformID.Win32NT
            ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(baseDir, InstallationDirName);
    }

    // Gets config path
    public static string GetConfigPath()
    {
        return Path.Combine(GetInstallationRoot(), ConfigFileName);
    }

    // Gets Versions directory path
    public static string GetVersionsDirectory()
    {
        return Path.Combine(GetInstallationRoot(), VersionsDirName);
    }

    // Gets path to a specific version's directory
    public static string GetVersionDirectory(string version)
    {
        return Path.Combine(GetVersionsDirectory(), version);
    }

    // gets path to a specific version's compiler executable
    public static string GetCompilerPath(string version)
    {
        var versionDir = GetVersionDirectory(version);
        var exeName = Environment.OSVersion.Platform == PlatformID.Win32NT
            ? "vecc-compiler.exe"
            : "vecc-compiler";

        return Path.Combine(versionDir, exeName);
    }

    public static string GetLauncherPath()
    {
        var exeName = Environment.OSVersion.Platform == PlatformID.Win32NT
            ? "vecc.exe"
            : "vecc";

        return Path.Combine(GetInstallationRoot(), exeName);
    }
}