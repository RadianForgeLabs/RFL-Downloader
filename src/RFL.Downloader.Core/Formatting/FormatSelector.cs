/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using RFL.Downloader.Models.Media;

namespace RFL.Downloader.Core.Formatting;

public class FormatSelector
{
    public static MediaFormat? SelectBestFormat(
        IReadOnlyList<MediaFormat> formats,
        string? resolutionPreference = null)
    {
        if (formats == null || formats.Count == 0)
            return null;

        var videoFormats = formats
            .Where(f => f.HasVideo == true)
            .OrderByDescending(f => f.Height ?? 0)
            .ThenByDescending(f => f.Fps ?? 0)
            .ThenByDescending(f => f.Vbr ?? f.Tbr ?? "0")
            .ToList();

        if (videoFormats.Count == 0)
            return formats.FirstOrDefault(f => f.HasAudio == true);

        if (!string.IsNullOrWhiteSpace(resolutionPreference))
        {
            var preferred = videoFormats.FirstOrDefault(f =>
                f.Resolution?.Equals(resolutionPreference, StringComparison.OrdinalIgnoreCase) == true);

            if (preferred != null)
                return preferred;
        }

        return videoFormats.FirstOrDefault();
    }

    public static MediaFormat? SelectBestVideoFormat(
        IReadOnlyList<MediaFormat> formats,
        string? resolutionPreference = null)
    {
        var videoOnlyFormats = formats
            .Where(f => f.OnlyVideo == true)
            .OrderByDescending(f => f.Height ?? 0)
            .ThenByDescending(f => f.Fps ?? 0)
            .ToList();

        if (videoOnlyFormats.Count == 0)
        {
            return SelectBestFormat(formats, resolutionPreference);
        }

        if (!string.IsNullOrWhiteSpace(resolutionPreference))
        {
            var preferred = videoOnlyFormats.FirstOrDefault(f =>
                f.Resolution?.Equals(resolutionPreference, StringComparison.OrdinalIgnoreCase) == true);

            if (preferred != null)
                return preferred;
        }

        return videoOnlyFormats.FirstOrDefault();
    }

    public static MediaFormat? SelectBestAudioFormat(
        IReadOnlyList<MediaFormat> formats)
    {
        var audioFormats = formats
            .Where(f => f.HasAudio == true)
            .OrderByDescending(f => f.AB ?? f.Tbr ?? "0")
            .ToList();

        return audioFormats.FirstOrDefault();
    }

    public static List<string> GetAvailableResolutions(IReadOnlyList<MediaFormat> formats)
    {
        return formats
            .Where(f => f.Height.HasValue && f.Height.Value > 0)
            .Select(f => $"{f.Height}p")
            .Distinct()
            .OrderByDescending(r => int.TryParse(r.Replace("p", ""), out var h) ? h : 0)
            .ToList();
    }

    public static List<string> GetAvailableContainers(IReadOnlyList<MediaFormat> formats)
    {
        return formats
            .Where(f => !string.IsNullOrWhiteSpace(f.Ext))
            .Select(f => f.Ext!.ToUpperInvariant())
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    public static bool RequiresMerging(
        IReadOnlyList<MediaFormat> formats,
        MediaFormat? selectedFormat)
    {
        if (selectedFormat == null)
            return false;

        return selectedFormat.VideoAndAudio == true ||
               (selectedFormat.HasVideo == true && selectedFormat.HasAudio == false &&
                formats.Any(f => f.OnlyAudio == true));
    }

    public static string? GetYtDlpFormatString(
        MediaFormat? videoFormat,
        MediaFormat? audioFormat,
        DownloadMode mode)
    {
        return mode switch
        {
            DownloadMode.VideoAudio when videoFormat != null && audioFormat != null =>
                $"{videoFormat.FormatId}+{audioFormat.FormatId}",
            DownloadMode.VideoAudio when videoFormat != null =>
                videoFormat.FormatId,
            DownloadMode.VideoOnly when videoFormat != null =>
                videoFormat.FormatId,
            DownloadMode.AudioOnly when audioFormat != null =>
                audioFormat.FormatId,
            DownloadMode.AudioOnly =>
                "bestaudio/best",
            DownloadMode.VideoOnly =>
                "bestvideo/best",
            _ => "best"
        };
    }
}
