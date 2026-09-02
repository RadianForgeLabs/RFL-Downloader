/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Models.Media;

public class MediaFormat
{
    public string FormatId { get; set; } = string.Empty;
    public string? Ext { get; set; }
    public string? Note { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? Resolution => Width.HasValue && Height.HasValue ? $"{Width}x{Height}" : null;
    public float? Fps { get; set; }
    public string? VCodec { get; set; }
    public string? ACodec { get; set; }
    public string? VExt { get; set; }
    public string? AExt { get; set; }
    public string? AB { get; set; }
    public string? Tbr { get; set; }
    public string? Vbr { get; set; }
    public string? Filesize { get; set; }
    public long? FilesizeApprox { get; set; }
    public string? FormatNote { get; set; }
    public string? Format { get; set; }
    public bool? HttpHeaders { get; set; }
    public bool? Preferred { get; set; }
    public string? Quality { get; set; }
    public bool? HasVideo { get; set; }
    public bool? HasAudio { get; set; }
    public bool? OnlyAudio { get; set; }
    public bool? OnlyVideo { get; set; }
    public bool? VideoAndAudio { get; set; }
    public bool? Hdr { get; set; }
    public string? DynamicRange { get; set; }
    public string? AudioChannels { get; set; }
    public string? Container => Ext;
}

public enum DownloadMode
{
    VideoAudio,
    VideoOnly,
    AudioOnly,
    SubtitlesOnly
}

public enum OutputFormat
{
    Mp4,
    Mkv,
    WebM,
    Mov,
    Avi,
    Mp3,
    M4A,
    Flac,
    Wav,
    Ogg,
    Opus,
    Srt,
    Vtt,
    Ass
}

public enum SubtitleFormat
{
    Srt,
    Vtt,
    Ass
}
