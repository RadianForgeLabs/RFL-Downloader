using RFL.Downloader.Models.Media;

/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Models.Downloads;

public class DownloadJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public string? Uploader { get; set; }
    public DownloadStatus Status { get; set; } = DownloadStatus.Waiting;
    public double Progress { get; set; }
    public long DownloadedBytes { get; set; }
    public long? TotalBytes { get; set; }
    public double Speed { get; set; }
    public TimeSpan? Eta { get; set; }
    public string? CurrentStage { get; set; }
    public string? OutputPath { get; set; }
    public DownloadMode Mode { get; set; } = DownloadMode.VideoAudio;
    public OutputFormat OutputFormat { get; set; } = OutputFormat.Mp4;
    public string? SelectedFormatId { get; set; }
    public string? VideoFormatId { get; set; }
    public string? AudioFormatId { get; set; }
    public string? Resolution { get; set; }
    public string? Fps { get; set; }
    public string? VideoCodec { get; set; }
    public string? AudioCodec { get; set; }
    public string? Container { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Engine { get; set; }
    public int RetryCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid? PresetId { get; set; }
    public string? CustomArguments { get; set; }
    public bool IsPaused { get; set; }
    public bool RequiresConversion { get; set; }
    public bool RequiresMerging { get; set; }
    public bool HasSubtitles { get; set; }
    public string? SubtitleLanguage { get; set; }
    public SubtitleFormat? SubtitleFormat { get; set; }
    public string? FilenameTemplate { get; set; }
    public string? OutputFolder { get; set; }
}

public enum DownloadStatus
{
    Waiting,
    Analyzing,
    Downloading,
    Processing,
    Merging,
    Converting,
    Finalizing,
    Completed,
    Failed,
    Cancelled,
    Paused
}
