/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Core.Abstractions;

public interface IBrowserIntegrationService
{
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    Task<BrowserRequest?> HandleBrowserRequestAsync(BrowserRequest request, CancellationToken cancellationToken = default);
    Task RegisterNativeHostAsync(CancellationToken cancellationToken = default);
    Task UnregisterNativeHostAsync(CancellationToken cancellationToken = default);
}

public class BrowserRequest
{
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? PresetName { get; set; }
    public BrowserAction Action { get; set; } = BrowserAction.Analyze;
    public string? Source { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public enum BrowserAction
{
    Analyze,
    Download,
    Queue
}
