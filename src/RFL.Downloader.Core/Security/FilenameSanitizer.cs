using System.Text;

/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Core.Security;

public static class FilenameSanitizer
{
    private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();
    private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();
    private static readonly string[] ReservedNames = new[]
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

    public static string SanitizeFilename(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return "download";

        var sanitized = new StringBuilder();
        var name = Path.GetFileNameWithoutExtension(filename);
        var extension = Path.GetExtension(filename);

        foreach (var c in name)
        {
            if (InvalidChars.Contains(c))
            {
                sanitized.Append('_');
            }
            else if (char.IsControl(c))
            {
                sanitized.Append('_');
            }
            else
            {
                sanitized.Append(c);
            }
        }

        string result = sanitized.ToString().Trim();

        if (string.IsNullOrWhiteSpace(result))
            result = "download";

        if (ReservedNames.Contains(result.ToUpperInvariant()))
            result = $"_{result}";

        if (!string.IsNullOrWhiteSpace(extension))
            result += extension;

        return result;
    }

    public static string SanitizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        var sanitized = new StringBuilder();

        foreach (var c in path)
        {
            if (InvalidPathChars.Contains(c))
            {
                sanitized.Append('_');
            }
            else if (char.IsControl(c))
            {
                sanitized.Append('_');
            }
            else
            {
                sanitized.Append(c);
            }
        }

        return sanitized.ToString();
    }

    public static bool IsValidPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        try
        {
            Path.GetFullPath(path);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsPathTraversal(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var normalized = path.Replace('/', '\\').Replace("..\\", "");
        return path.Contains("..") && !normalized.Equals(path);
    }

    public static string SafeCombinePath(string basePath, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
            throw new ArgumentException("Base path cannot be empty", nameof(basePath));

        if (string.IsNullOrWhiteSpace(relativePath))
            return basePath;

        if (IsPathTraversal(relativePath))
            throw new ArgumentException("Path traversal detected", nameof(relativePath));

        try
        {
            return Path.GetFullPath(Path.Combine(basePath, relativePath));
        }
        catch
        {
            return basePath;
        }
    }
}
