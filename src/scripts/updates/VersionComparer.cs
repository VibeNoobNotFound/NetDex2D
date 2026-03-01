using System;

namespace NetDex.Updates;

public static class VersionComparer
{
    public static bool IsNewer(string candidateVersion, string currentVersion)
    {
        return Compare(candidateVersion, currentVersion) > 0;
    }

    public static int Compare(string leftVersion, string rightVersion)
    {
        var left = Parse(leftVersion);
        var right = Parse(rightVersion);

        if (left.Major != right.Major)
        {
            return left.Major.CompareTo(right.Major);
        }

        if (left.Minor != right.Minor)
        {
            return left.Minor.CompareTo(right.Minor);
        }

        return left.Patch.CompareTo(right.Patch);
    }

    public static string NormalizeTagToVersion(string tag)
    {
        var parsed = Parse(tag);
        return $"{parsed.Major}.{parsed.Minor}.{parsed.Patch}";
    }

    private static (int Major, int Minor, int Patch) Parse(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return (0, 0, 0);
        }

        var cleaned = raw.Trim();
        if (cleaned.StartsWith("v", StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned[1..];
        }

        var prereleaseIndex = cleaned.IndexOf('-', StringComparison.Ordinal);
        if (prereleaseIndex >= 0)
        {
            cleaned = cleaned[..prereleaseIndex];
        }

        var buildIndex = cleaned.IndexOf('+', StringComparison.Ordinal);
        if (buildIndex >= 0)
        {
            cleaned = cleaned[..buildIndex];
        }

        var parts = cleaned.Split('.', StringSplitOptions.RemoveEmptyEntries);

        var major = ParsePart(parts, 0);
        var minor = ParsePart(parts, 1);
        var patch = ParsePart(parts, 2);

        return (major, minor, patch);
    }

    private static int ParsePart(string[] parts, int index)
    {
        if (index >= parts.Length)
        {
            return 0;
        }

        return int.TryParse(parts[index], out var parsed) ? parsed : 0;
    }
}
