using System.Text.RegularExpressions;

/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Core.Validation;

public static class UrlValidator
{
    private static readonly Regex UrlRegex = new(
        @"^(https?|ftp)://[^\s/$.?#].[^\s]*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex YoutubeRegex = new(
        @"^(https?://)?(www\.)?(youtube\.com|youtu\.?be)/.+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex CommonMediaRegex = new(
        @"^(https?://)?(www\.)?.+\.(mp4|webm|mkv|mov|avi|flv|wmv|mp3|wav|ogg|m4a|flac)(\?.*)?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool IsValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
               && UrlRegex.IsMatch(url);
    }

    public static bool IsYoutubeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return YoutubeRegex.IsMatch(url);
    }

    public static bool IsDirectMediaUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return CommonMediaRegex.IsMatch(url);
    }

    public static string? SanitizeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        try
        {
            var uri = new Uri(url);
            return uri.ToString();
        }
        catch
        {
            return null;
        }
    }

    public static string? ExtractDomain(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        try
        {
            var uri = new Uri(url);
            return uri.Host;
        }
        catch
        {
            return null;
        }
    }
}
