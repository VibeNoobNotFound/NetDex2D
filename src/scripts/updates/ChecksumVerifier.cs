using System;
using System.IO;
using System.Security.Cryptography;
using Godot;

namespace NetDex.Updates;

public static class ChecksumVerifier
{
    public static bool TryParseSha256Digest(string digest, out string expectedHex)
    {
        expectedHex = string.Empty;
        if (string.IsNullOrWhiteSpace(digest))
        {
            return false;
        }

        const string prefix = "sha256:";
        if (!digest.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var hex = digest[prefix.Length..].Trim();
        if (hex.Length != 64)
        {
            return false;
        }

        foreach (var ch in hex)
        {
            if (!Uri.IsHexDigit(ch))
            {
                return false;
            }
        }

        expectedHex = hex.ToLowerInvariant();
        return true;
    }

    public static string ComputeSha256Hex(string godotPath)
    {
        var globalPath = ProjectSettings.GlobalizePath(godotPath);
        if (!File.Exists(globalPath))
        {
            throw new FileNotFoundException("File not found for checksum.", globalPath);
        }

        using var stream = File.OpenRead(globalPath);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static bool VerifyFileSha256(string godotPath, string expectedHex, out string actualHex)
    {
        actualHex = ComputeSha256Hex(godotPath);
        return string.Equals(actualHex, expectedHex.Trim().ToLowerInvariant(), StringComparison.Ordinal);
    }
}
