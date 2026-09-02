/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.EntityFrameworkCore;
using RFL.Downloader.Infrastructure.Data;
using RFL.Downloader.Models.Downloads;

namespace RFL.Downloader.Infrastructure.Repositories;

public class DownloadJobRepository
{
    private readonly DownloaderDbContext _context;

    public DownloadJobRepository(DownloaderDbContext context)
    {
        _context = context;
    }

    public async Task<DownloadJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DownloadJobs
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DownloadJob>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DownloadJobs
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DownloadJob>> GetByStatusAsync(DownloadStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.DownloadJobs
            .Where(j => j.Status == status)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DownloadJob>> GetActiveDownloadsAsync(CancellationToken cancellationToken = default)
    {
        var activeStatuses = new[] { DownloadStatus.Downloading, DownloadStatus.Processing, DownloadStatus.Merging, DownloadStatus.Converting };
        return await _context.DownloadJobs
            .Where(j => activeStatuses.Contains(j.Status))
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DownloadJob>> GetQueuedDownloadsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DownloadJobs
            .Where(j => j.Status == DownloadStatus.Waiting)
            .OrderBy(j => j.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(DownloadJob job, CancellationToken cancellationToken = default)
    {
        await _context.DownloadJobs.AddAsync(job, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(DownloadJob job, CancellationToken cancellationToken = default)
    {
        _context.DownloadJobs.Update(job);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var job = await GetByIdAsync(id, cancellationToken);
        if (job != null)
        {
            _context.DownloadJobs.Remove(job);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DownloadJobs.CountAsync(cancellationToken);
    }

    public async Task<int> GetCountByStatusAsync(DownloadStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.DownloadJobs
            .CountAsync(j => j.Status == status, cancellationToken);
    }
}
