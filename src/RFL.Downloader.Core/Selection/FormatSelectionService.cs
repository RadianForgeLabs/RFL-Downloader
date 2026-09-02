/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using RFL.Downloader.Models.Media;
using RFL.Downloader.Core.Abstractions;
using RFL.Downloader.Core.Formatting;

namespace RFL.Downloader.Core.Selection;

public class FormatSelectionService : IFormatAnalyzer
{
    public Task<FormatAnalysisResult> AnalyzeFormatsAsync(IReadOnlyList<MediaFormat> formats, DownloadMode mode, CancellationToken cancellationToken = default)
    {
        var result = new FormatAnalysisResult
        {
            AvailableFormats = formats.ToList(),
            AvailableResolutions = FormatSelector.GetAvailableResolutions(formats),
            AvailableContainers = FormatSelector.GetAvailableContainers(formats),
            HasVideo = formats.Any(f => f.HasVideo == true),
            HasAudio = formats.Any(f => f.HasAudio == true),
            HasSubtitles = mode == DownloadMode.SubtitlesOnly
        };

        result.RequiresMerging = result.HasVideo && result.HasAudio && mode == DownloadMode.VideoAudio;
        result.RequiresConversion = ShouldRequireConversion(formats, mode);

        return Task.FromResult(result);
    }

    public Task<MediaFormat?> SelectBestFormatAsync(IReadOnlyList<MediaFormat> formats, string? resolutionPreference = null, CancellationToken cancellationToken = default)
    {
        var bestFormat = FormatSelector.SelectBestFormat(formats, resolutionPreference);
        return Task.FromResult(bestFormat);
    }

    public Task<IReadOnlyList<MediaFormat>> FilterFormatsAsync(IReadOnlyList<MediaFormat> formats, DownloadMode mode, CancellationToken cancellationToken = default)
    {
        var filtered = mode switch
        {
            DownloadMode.VideoOnly => formats.Where(f => f.HasVideo == true).ToList(),
            DownloadMode.AudioOnly => formats.Where(f => f.HasAudio == true).ToList(),
            DownloadMode.VideoAudio => formats.Where(f => f.HasVideo == true || f.HasAudio == true).ToList(),
            DownloadMode.SubtitlesOnly => new List<MediaFormat>(),
            _ => formats.ToList()
        };

        return Task.FromResult<IReadOnlyList<MediaFormat>>(filtered);
    }

    private static bool ShouldRequireConversion(IReadOnlyList<MediaFormat> formats, DownloadMode mode)
    {
        if (mode == DownloadMode.SubtitlesOnly)
            return false;

        var hasMp4 = formats.Any(f => f.Ext?.Equals("mp4", StringComparison.OrdinalIgnoreCase) == true);
        var hasMkv = formats.Any(f => f.Ext?.Equals("mkv", StringComparison.OrdinalIgnoreCase) == true);

        return !hasMp4 && !hasMkv;
    }
}
