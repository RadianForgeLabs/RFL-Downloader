/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Models.Settings;

public class AppSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ApplicationTheme Theme { get; set; } = ApplicationTheme.System;
    public string? Language { get; set; }
    public bool StartWithWindows { get; set; }
    public bool MinimizeToTray { get; set; }
    public bool ShowNotifications { get; set; } = true;
    public bool CheckForUpdates { get; set; } = true;
    public DateTime LastUpdateCheck { get; set; } = DateTime.MinValue;
    public string? Version { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum ApplicationTheme
{
    Light,
    Dark,
    System
}
