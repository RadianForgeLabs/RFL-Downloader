/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using RFL.Downloader.Models.Downloads;
using RFL.Downloader.Models.Engines;
using RFL.Downloader.Models.Media;

namespace RFL.Downloader.Core.Abstractions;

public interface IDownloadEngine
{
    string Name { get; }
    string Version { get; }
    bool IsAvailable { get; }

    Task<EngineInfo> GetEngineInfoAsync(CancellationToken cancellationToken = default);
    Task<MediaInfo> AnalyzeAsync(string url, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MediaFormat>> GetFormatsAsync(string url, CancellationToken cancellationToken = default);
    Task<DownloadJob> DownloadAsync(DownloadJob job, IProgress<DownloadProgress>? progress = null, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<bool> CheckForUpdateAsync(CancellationToken cancellationToken = default);
    Task<string?> GetVersionAsync(CancellationToken cancellationToken = default);
}

public class DownloadProgress
{
    public double Percentage { get; set; }
    public long DownloadedBytes { get; set; }
    public long? TotalBytes { get; set; }
    public double Speed { get; set; }
    public TimeSpan? Eta { get; set; }
    public string? CurrentStage { get; set; }
    public string? StatusMessage { get; set; }
}
