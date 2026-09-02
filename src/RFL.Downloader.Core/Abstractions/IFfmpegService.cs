using RFL.Downloader.Models.Engines;
using RFL.Downloader.Models.Media;

/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Core.Abstractions;

public interface IFfmpegService
{
    string Name { get; }
    bool IsAvailable { get; }

    Task<EngineInfo> GetEngineInfoAsync(CancellationToken cancellationToken = default);
    Task<string?> GetVersionAsync(CancellationToken cancellationToken = default);
    Task<bool> CheckForUpdateAsync(CancellationToken cancellationToken = default);
    Task MergeStreamsAsync(string videoPath, string audioPath, string outputPath, IProgress<ConversionProgress>? progress = null, CancellationToken cancellationToken = default);
    Task ConvertFormatAsync(string inputPath, string outputPath, string? format = null, IProgress<ConversionProgress>? progress = null, CancellationToken cancellationToken = default);
    Task<MediaInfo?> GetMediaInfoAsync(string filePath, CancellationToken cancellationToken = default);
    Task CancelAsync(CancellationToken cancellationToken = default);
}

public class ConversionProgress
{
    public double Percentage { get; set; }
    public string? CurrentStage { get; set; }
    public string? StatusMessage { get; set; }
    public TimeSpan? Eta { get; set; }
    public long ProcessedBytes { get; set; }
    public long? TotalBytes { get; set; }
}
