using RFL.Downloader.Models.Downloads;

/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Core.Abstractions;

public interface IDownloadService
{
    Task<DownloadJob> CreateDownloadJobAsync(string url, CancellationToken cancellationToken = default);
    Task<DownloadJob?> GetDownloadJobAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DownloadJob>> GetAllDownloadJobsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DownloadJob>> GetActiveDownloadsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DownloadJob>> GetQueuedDownloadsAsync(CancellationToken cancellationToken = default);
    Task<DownloadJob> UpdateDownloadJobAsync(DownloadJob job, CancellationToken cancellationToken = default);
    Task DeleteDownloadJobAsync(Guid id, CancellationToken cancellationToken = default);
    Task PauseDownloadAsync(Guid id, CancellationToken cancellationToken = default);
    Task ResumeDownloadAsync(Guid id, CancellationToken cancellationToken = default);
    Task CancelDownloadAsync(Guid id, CancellationToken cancellationToken = default);
    Task RetryDownloadAsync(Guid id, CancellationToken cancellationToken = default);
}
