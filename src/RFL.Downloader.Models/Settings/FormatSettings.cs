using RFL.Downloader.Models.Media;

/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Models.Settings;

public class FormatSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? PreferredResolution { get; set; }
    public OutputFormat? PreferredContainer { get; set; } = OutputFormat.Mp4;
    public string? PreferredVideoCodec { get; set; }
    public string? PreferredAudioCodec { get; set; }
    public DownloadMode DefaultMode { get; set; } = DownloadMode.VideoAudio;
    public bool PreferHdr { get; set; }
    public bool PreferHighFps { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
