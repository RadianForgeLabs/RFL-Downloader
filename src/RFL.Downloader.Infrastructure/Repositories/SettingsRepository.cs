/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.EntityFrameworkCore;
using RFL.Downloader.Infrastructure.Data;
using RFL.Downloader.Models.Settings;

namespace RFL.Downloader.Infrastructure.Repositories;

public class SettingsRepository
{
    private readonly DownloaderDbContext _context;

    public SettingsRepository(DownloaderDbContext context)
    {
        _context = context;
    }

    public async Task<AppSettings> GetAppSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _context.AppSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings == null)
        {
            settings = new AppSettings();
            await _context.AppSettings.AddAsync(settings, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return settings;
    }

    public async Task SaveAppSettingsAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        _context.AppSettings.Update(settings);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<DownloadSettings> GetDownloadSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _context.DownloadSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings == null)
        {
            settings = new DownloadSettings();
            await _context.DownloadSettings.AddAsync(settings, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return settings;
    }

    public async Task SaveDownloadSettingsAsync(DownloadSettings settings, CancellationToken cancellationToken = default)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        _context.DownloadSettings.Update(settings);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<FormatSettings> GetFormatSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _context.FormatSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings == null)
        {
            settings = new FormatSettings();
            await _context.FormatSettings.AddAsync(settings, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return settings;
    }

    public async Task SaveFormatSettingsAsync(FormatSettings settings, CancellationToken cancellationToken = default)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        _context.FormatSettings.Update(settings);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<EngineSettings> GetEngineSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _context.EngineSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings == null)
        {
            settings = new EngineSettings();
            await _context.EngineSettings.AddAsync(settings, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return settings;
    }

    public async Task SaveEngineSettingsAsync(EngineSettings settings, CancellationToken cancellationToken = default)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        _context.EngineSettings.Update(settings);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AdvancedSettings> GetAdvancedSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _context.AdvancedSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings == null)
        {
            settings = new AdvancedSettings();
            await _context.AdvancedSettings.AddAsync(settings, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return settings;
    }

    public async Task SaveAdvancedSettingsAsync(AdvancedSettings settings, CancellationToken cancellationToken = default)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        _context.AdvancedSettings.Update(settings);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
