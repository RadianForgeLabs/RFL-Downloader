/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Models.Engines;

public class EngineInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string ExecutablePath { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public bool IsBundled { get; set; }
    public DateTime? LastUpdateCheck { get; set; }
    public string? LatestVersion { get; set; }
    public bool UpdateAvailable { get; set; }
}

public class EngineUpdateInfo
{
    public string CurrentVersion { get; set; } = string.Empty;
    public string LatestVersion { get; set; } = string.Empty;
    public bool UpdateAvailable { get; set; }
    public string? DownloadUrl { get; set; }
    public string? ReleaseNotes { get; set; }
    public DateTime ReleaseDate { get; set; }
}
