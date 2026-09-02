/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Models.Media;

public class MediaInfo
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Uploader { get; set; } = string.Empty;
    public string UploaderId { get; set; } = string.Empty;
    public string UploaderUrl { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string ChannelId { get; set; } = string.Empty;
    public string ChannelUrl { get; set; } = string.Empty;
    public string WebpageUrl { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime? UploadDate { get; set; }
    public string? ViewCount { get; set; }
    public string? LikeCount { get; set; }
    public string? Category { get; set; }
    public List<MediaFormat> Formats { get; set; } = new();
    public PlaylistInfo? Playlist { get; set; }
    public string? Subtitles { get; set; }
    public int FormatCount => Formats.Count;
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}

public class PlaylistInfo
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Uploader { get; set; }
    public int? Index { get; set; }
    public int? Count { get; set; }
    public string? Url { get; set; }
}
