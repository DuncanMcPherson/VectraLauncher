namespace VectraLauncher.Models;

/// <summary>
/// Represents the configuration for the Vectra Launcher, including compiler versioning and update settings.
/// </summary>
internal class VectraConfiguration
{
    /// <summary>
    /// The active compiler version. Used when running a compiler command.
    /// Possible values:
    /// - A specific version (e.g. "1.2.3")
    /// - "latest" (default), which points to the installed version with the highest version number.
    /// </summary>
    public string ActiveVersion { get; set; } = "latest";

    public string ResolvedActiveVersion { get; set; } = string.Empty;

    /// <summary>
    /// The highest installed compiler version detected on the system.
    /// </summary>
    public string LatestInstalledVersion { get; set; } = string.Empty;

    /// <summary>
    /// Frequency (in days) to check for compiler updates automatically.
    /// </summary>
    public int AutoUpdateCheckFrequencyDays { get; set; } = 7;

    /// <summary>
    /// The date and time of the last update check. Null if never checked.
    /// </summary>
    public DateTime? LastUpdateCheck { get; set; }

    /// <summary>
    /// List of all installed compiler versions and their paths.
    /// </summary>
    public List<InstalledVersion> InstalledVersions { get; set; } = [];
}

/// <summary>
/// Represents an installed compiler version and its file path.
/// </summary>
internal class InstalledVersion
{
    /// <summary>
    /// The version string (e.g. "1.2.3") of the installed compiler.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// The file system path to the installed compiler version.
    /// </summary>
    public string Path { get; set; } = string.Empty;
}
