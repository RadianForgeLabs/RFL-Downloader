using RFL.Downloader.Models.Media;

/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Core.Abstractions;

public interface IFormatAnalyzer
{
    Task<FormatAnalysisResult> AnalyzeFormatsAsync(IReadOnlyList<MediaFormat> formats, DownloadMode mode, CancellationToken cancellationToken = default);
    Task<MediaFormat?> SelectBestFormatAsync(IReadOnlyList<MediaFormat> formats, string? resolutionPreference = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MediaFormat>> FilterFormatsAsync(IReadOnlyList<MediaFormat> formats, DownloadMode mode, CancellationToken cancellationToken = default);
}

public class FormatAnalysisResult
{
    public List<MediaFormat> AvailableFormats { get; set; } = new();
    public List<string> AvailableResolutions { get; set; } = new();
    public List<string> AvailableContainers { get; set; } = new();
    public bool HasVideo { get; set; }
    public bool HasAudio { get; set; }
    public bool HasSubtitles { get; set; }
    public bool RequiresMerging { get; set; }
    public bool RequiresConversion { get; set; }
}
