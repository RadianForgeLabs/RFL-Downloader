/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Models.Errors;

public class DownloadError
{
    public ErrorType ErrorType { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? TechnicalDetails { get; set; }
    public string? DiagnosticInfo { get; set; }
    public bool CanRetry { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public enum ErrorType
{
    InvalidUrl,
    UnsupportedSite,
    NoFormatsAvailable,
    NetworkError,
    AuthenticationRequired,
    RateLimited,
    EngineMissing,
    EngineError,
    FFmpegMissing,
    FFmpegError,
    DiskFull,
    PermissionDenied,
    Cancelled,
    UnknownError
}
