namespace VectraLauncher.Models;

internal readonly struct SemanticVersion : IComparable<SemanticVersion>
{
    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }

    private SemanticVersion(int major, int minor, int patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    public static bool TryParse(string version, out SemanticVersion result)
    {
        result = default;

        if (string.IsNullOrEmpty(version))
            return false;
        var parts = version.Split('.');
        if (parts.Length != 3)
            return false;
        if (!int.TryParse(parts[0], out var major) || major < 0)
            return false;
        if (!int.TryParse(parts[1], out var minor) || minor < 0)
            return false;
        if (!int.TryParse(parts[2], out var patch) || patch < 0)
            return false;
        result = new SemanticVersion(major, minor, patch);
        return true;
    }

    public int CompareTo(SemanticVersion other)
    {
        var majorVersion = Major.CompareTo(other.Major);
        if (majorVersion != 0)
            return majorVersion;
        var minorVersion = Minor.CompareTo(other.Minor);
        if (minorVersion != 0)
            return minorVersion;
        return Patch.CompareTo(other.Patch);
    }

    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}