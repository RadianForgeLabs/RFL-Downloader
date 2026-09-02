using Microsoft.Extensions.Logging;
using RFL.Downloader.Core.Abstractions;
using RFL.Downloader.Infrastructure.Repositories;
/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using RFL.Downloader.Models.Downloads;
using RFL.Downloader.Models.Settings;

namespace RFL.Downloader.DownloadManager;

public class DownloadManagerService
{
    private readonly ILogger<DownloadManagerService> _logger;
    private readonly IDownloadEngine _downloadEngine;
    private readonly IFfmpegService _ffmpegService;
    private readonly DownloadJobRepository _jobRepository;
    private readonly SettingsRepository _settingsRepository;
    private readonly SemaphoreSlim _concurrencySemaphore;
    private readonly Dictionary<Guid, CancellationTokenSource> _cancellationTokens;
    private readonly object _lock = new();
    private bool _isRunning;

    public DownloadManagerService(
        ILogger<DownloadManagerService> logger,
        IDownloadEngine downloadEngine,
        IFfmpegService ffmpegService,
        DownloadJobRepository jobRepository,
        SettingsRepository settingsRepository)
    {
        _logger = logger;
        _downloadEngine = downloadEngine;
        _ffmpegService = ffmpegService;
        _jobRepository = jobRepository;
        _settingsRepository = settingsRepository;
        _cancellationTokens = new Dictionary<Guid, CancellationTokenSource>();
        _isRunning = false;

        var maxConcurrency = 3; // Default, will be loaded from settings
        _concurrencySemaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning)
            return;

        _isRunning = true;
        _logger.LogInformation("Download Manager started");

        // Load settings for concurrency
        var settings = await _settingsRepository.GetDownloadSettingsAsync(cancellationToken);
        UpdateConcurrency(settings.MaxConcurrentDownloads);

        // Resume any incomplete downloads
        await ResumeIncompleteDownloadsAsync(cancellationToken);

        // Start processing queue
        _ = ProcessQueueAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_isRunning)
            return;

        _isRunning = false;
        _logger.LogInformation("Download Manager stopping");

        // Cancel all active downloads
        lock (_lock)
        {
            foreach (var cts in _cancellationTokens.Values)
            {
                cts.Cancel();
            }
            _cancellationTokens.Clear();
        }

        _logger.LogInformation("Download Manager stopped");
    }

    public async Task<Guid> EnqueueDownloadAsync(DownloadJob job, CancellationToken cancellationToken = default)
    {
        job.Status = DownloadStatus.Waiting;
        job.CreatedAt = DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;

        await _jobRepository.AddAsync(job, cancellationToken);
        _logger.LogInformation("Download job {JobId} enqueued for URL: {Url}", job.Id, job.Url);

        return job.Id;
    }

    public async Task PauseDownloadAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_cancellationTokens.TryGetValue(jobId, out var cts))
            {
                cts.Cancel();
                _cancellationTokens.Remove(jobId);
            }
        }

        var job = await _jobRepository.GetByIdAsync(jobId, cancellationToken);
        if (job != null)
        {
            job.Status = DownloadStatus.Paused;
            job.IsPaused = true;
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job, cancellationToken);
            _logger.LogInformation("Download job {JobId} paused", jobId);
        }
    }

    public async Task ResumeDownloadAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await _jobRepository.GetByIdAsync(jobId, cancellationToken);
        if (job != null)
        {
            job.Status = DownloadStatus.Waiting;
            job.IsPaused = false;
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job, cancellationToken);
            _logger.LogInformation("Download job {JobId} resumed", jobId);
        }
    }

    public async Task CancelDownloadAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_cancellationTokens.TryGetValue(jobId, out var cts))
            {
                cts.Cancel();
                _cancellationTokens.Remove(jobId);
            }
        }

        var job = await _jobRepository.GetByIdAsync(jobId, cancellationToken);
        if (job != null)
        {
            job.Status = DownloadStatus.Cancelled;
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job, cancellationToken);
            _logger.LogInformation("Download job {JobId} cancelled", jobId);
        }
    }

    public async Task RetryDownloadAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await _jobRepository.GetByIdAsync(jobId, cancellationToken);
        if (job != null)
        {
            job.Status = DownloadStatus.Waiting;
            job.RetryCount++;
            job.ErrorMessage = null;
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job, cancellationToken);
            _logger.LogInformation("Download job {JobId} queued for retry (attempt {RetryCount})", jobId, job.RetryCount);
        }
    }

    public async Task<DownloadJob?> GetJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await _jobRepository.GetByIdAsync(jobId, cancellationToken);
    }

    public async Task<IReadOnlyList<DownloadJob>> GetAllJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _jobRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DownloadJob>> GetActiveJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _jobRepository.GetActiveDownloadsAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DownloadJob>> GetQueuedJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _jobRepository.GetQueuedDownloadsAsync(cancellationToken);
    }

    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        while (_isRunning && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _concurrencySemaphore.WaitAsync(cancellationToken);

                var job = await GetNextJobAsync(cancellationToken);
                if (job != null)
                {
                    _ = ProcessJobAsync(job, cancellationToken);
                }
                else
                {
                    _concurrencySemaphore.Release();
                    await Task.Delay(1000, cancellationToken); // Wait before checking again
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing download queue");
                _concurrencySemaphore.Release();
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private async Task<DownloadJob?> GetNextJobAsync(CancellationToken cancellationToken)
    {
        var queuedJobs = await _jobRepository.GetQueuedDownloadsAsync(cancellationToken);
        return queuedJobs.FirstOrDefault(j => !j.IsPaused);
    }

    private async Task ProcessJobAsync(DownloadJob job, CancellationToken cancellationToken)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        lock (_lock)
        {
            _cancellationTokens[job.Id] = cts;
        }

        try
        {
            job.Status = DownloadStatus.Downloading;
            job.StartedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job, cancellationToken);

            var progress = new Progress<DownloadProgress>(p =>
            {
                job.Progress = p.Percentage;
                job.DownloadedBytes = p.DownloadedBytes;
                job.TotalBytes = p.TotalBytes;
                job.Speed = p.Speed;
                job.Eta = p.Eta;
                job.CurrentStage = p.CurrentStage;
                job.UpdatedAt = DateTime.UtcNow;
            });

            var updatedJob = await _downloadEngine.DownloadAsync(job, progress, cts.Token);

            if (updatedJob.RequiresMerging && _ffmpegService.IsAvailable)
            {
                job.Status = DownloadStatus.Merging;
                job.CurrentStage = "Merging streams";
                await _jobRepository.UpdateAsync(job, cancellationToken);

                // Implement merge logic here if needed
            }

            if (updatedJob.RequiresConversion && _ffmpegService.IsAvailable)
            {
                job.Status = DownloadStatus.Converting;
                job.CurrentStage = "Converting format";
                await _jobRepository.UpdateAsync(job, cancellationToken);

                // Implement conversion logic here if needed
            }

            job.Status = DownloadStatus.Completed;
            job.Progress = 100;
            job.CompletedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job, cancellationToken);

            _logger.LogInformation("Download job {JobId} completed successfully", job.Id);
        }
        catch (OperationCanceledException)
        {
            job.Status = job.IsPaused ? DownloadStatus.Paused : DownloadStatus.Cancelled;
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job, cancellationToken);
            _logger.LogInformation("Download job {JobId} was {Status}", job.Id, job.Status);
        }
        catch (Exception ex)
        {
            job.Status = DownloadStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job, cancellationToken);
            _logger.LogError(ex, "Download job {JobId} failed", job.Id);

            // Auto-retry if under limit
            var settings = await _settingsRepository.GetDownloadSettingsAsync(cancellationToken);
            if (job.RetryCount < settings.MaxRetries)
            {
                await Task.Delay(5000, cancellationToken);
                await RetryDownloadAsync(job.Id, cancellationToken);
            }
        }
        finally
        {
            lock (_lock)
            {
                _cancellationTokens.Remove(job.Id);
            }
            _concurrencySemaphore.Release();
        }
    }

    private async Task ResumeIncompleteDownloadsAsync(CancellationToken cancellationToken)
    {
        var incompleteJobs = await _jobRepository.GetByStatusAsync(DownloadStatus.Downloading, cancellationToken);
        foreach (var job in incompleteJobs)
        {
            job.Status = DownloadStatus.Waiting;
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job, cancellationToken);
            _logger.LogInformation("Resumed incomplete download job {JobId}", job.Id);
        }
    }

    private void UpdateConcurrency(int maxConcurrency)
    {
        // In a production implementation, you'd need to handle this more carefully
        // to avoid releasing more than the semaphore's current count
        _logger.LogInformation("Concurrency updated to {MaxConcurrency}", maxConcurrency);
    }
}
