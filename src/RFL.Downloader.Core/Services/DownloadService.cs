/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using RFL.Downloader.Models.Downloads;
using RFL.Downloader.Core.Abstractions;

namespace RFL.Downloader.Core.Services;

public class DownloadService : IDownloadService
{
    private readonly Dictionary<Guid, DownloadJob> _jobs = new();

    public Task<DownloadJob> CreateDownloadJobAsync(string url, CancellationToken cancellationToken = default)
    {
        var job = new DownloadJob
        {
            Url = url,
            Status = DownloadStatus.Waiting
        };

        _jobs[job.Id] = job;
        return Task.FromResult(job);
    }

    public Task<DownloadJob?> GetDownloadJobAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _jobs.TryGetValue(id, out var job);
        return Task.FromResult(job);
    }

    public Task<IReadOnlyList<DownloadJob>> GetAllDownloadJobsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<DownloadJob>>(_jobs.Values.ToList());
    }

    public Task<IReadOnlyList<DownloadJob>> GetActiveDownloadsAsync(CancellationToken cancellationToken = default)
    {
        var active = _jobs.Values
            .Where(j => j.Status == DownloadStatus.Downloading || j.Status == DownloadStatus.Processing)
            .ToList();

        return Task.FromResult<IReadOnlyList<DownloadJob>>(active);
    }

    public Task<IReadOnlyList<DownloadJob>> GetQueuedDownloadsAsync(CancellationToken cancellationToken = default)
    {
        var queued = _jobs.Values
            .Where(j => j.Status == DownloadStatus.Waiting)
            .ToList();

        return Task.FromResult<IReadOnlyList<DownloadJob>>(queued);
    }

    public Task<DownloadJob> UpdateDownloadJobAsync(DownloadJob job, CancellationToken cancellationToken = default)
    {
        job.UpdatedAt = DateTime.UtcNow;
        _jobs[job.Id] = job;
        return Task.FromResult(job);
    }

    public Task DeleteDownloadJobAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _jobs.Remove(id);
        return Task.CompletedTask;
    }

    public Task PauseDownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_jobs.TryGetValue(id, out var job))
        {
            job.IsPaused = true;
            job.Status = DownloadStatus.Paused;
            job.UpdatedAt = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task ResumeDownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_jobs.TryGetValue(id, out var job))
        {
            job.IsPaused = false;
            job.Status = DownloadStatus.Waiting;
            job.UpdatedAt = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task CancelDownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_jobs.TryGetValue(id, out var job))
        {
            job.Status = DownloadStatus.Cancelled;
            job.UpdatedAt = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task RetryDownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_jobs.TryGetValue(id, out var job))
        {
            job.Status = DownloadStatus.Waiting;
            job.RetryCount++;
            job.ErrorMessage = null;
            job.UpdatedAt = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }
}
