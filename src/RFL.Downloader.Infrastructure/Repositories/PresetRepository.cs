/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.EntityFrameworkCore;
using RFL.Downloader.Infrastructure.Data;
using RFL.Downloader.Models.Presets;
using RFL.Downloader.Models.Media;

namespace RFL.Downloader.Infrastructure.Repositories;

public class PresetRepository
{
    private readonly DownloaderDbContext _context;

    public PresetRepository(DownloaderDbContext context)
    {
        _context = context;
    }

    public async Task<Preset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Presets
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Preset>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Presets
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Preset>> GetBuiltInPresetsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Presets
            .Where(p => p.IsBuiltIn)
            .OrderBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Preset>> GetCustomPresetsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Presets
            .Where(p => !p.IsBuiltIn)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Preset preset, CancellationToken cancellationToken = default)
    {
        await _context.Presets.AddAsync(preset, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Preset preset, CancellationToken cancellationToken = default)
    {
        _context.Presets.Update(preset);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var preset = await GetByIdAsync(id, cancellationToken);
        if (preset != null && !preset.IsBuiltIn)
        {
            _context.Presets.Remove(preset);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task EnsureBuiltInPresetsAsync(CancellationToken cancellationToken = default)
    {
        var existingBuiltIn = await GetBuiltInPresetsAsync(cancellationToken);
        if (existingBuiltIn.Any())
            return;

        var builtInPresets = GetDefaultBuiltInPresets();
        await _context.Presets.AddRangeAsync(builtInPresets, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static List<Preset> GetDefaultBuiltInPresets()
    {
        return new List<Preset>
        {
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Best Quality",
                Description = "Highest available quality with preferred container",
                IsBuiltIn = true,
                Mode = DownloadMode.VideoAudio,
                ResolutionPreference = "2160p",
                PreferredContainer = OutputFormat.Mp4,
                IncludeMetadata = true,
                IncludeThumbnail = true,
                SortOrder = 1
            },
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "1080p MP4",
                Description = "1080p video in MP4 container",
                IsBuiltIn = true,
                Mode = DownloadMode.VideoAudio,
                ResolutionPreference = "1080p",
                PreferredContainer = OutputFormat.Mp4,
                IncludeMetadata = true,
                IncludeThumbnail = true,
                SortOrder = 2
            },
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "720p MP4",
                Description = "720p video in MP4 container",
                IsBuiltIn = true,
                Mode = DownloadMode.VideoAudio,
                ResolutionPreference = "720p",
                PreferredContainer = OutputFormat.Mp4,
                IncludeMetadata = true,
                IncludeThumbnail = true,
                SortOrder = 3
            },
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Name = "Best Audio MP3",
                Description = "Best quality audio in MP3 format",
                IsBuiltIn = true,
                Mode = DownloadMode.AudioOnly,
                PreferredContainer = OutputFormat.Mp3,
                IncludeMetadata = true,
                SortOrder = 4
            },
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                Name = "Best Audio FLAC",
                Description = "Best quality audio in FLAC format",
                IsBuiltIn = true,
                Mode = DownloadMode.AudioOnly,
                PreferredContainer = OutputFormat.Flac,
                IncludeMetadata = true,
                SortOrder = 5
            },
            new Preset
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                Name = "Archive MKV",
                Description = "Best quality in MKV container for archiving",
                IsBuiltIn = true,
                Mode = DownloadMode.VideoAudio,
                PreferredContainer = OutputFormat.Mkv,
                IncludeMetadata = true,
                IncludeThumbnail = true,
                SortOrder = 6
            }
        };
    }
}
