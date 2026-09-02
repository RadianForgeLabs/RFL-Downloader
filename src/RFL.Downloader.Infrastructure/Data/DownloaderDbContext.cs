/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.EntityFrameworkCore;
using RFL.Downloader.Models.Downloads;
using RFL.Downloader.Models.Presets;
using RFL.Downloader.Models.Settings;
using RFL.Downloader.Models.Media;

namespace RFL.Downloader.Infrastructure.Data;

public class DownloaderDbContext : DbContext
{
    public DbSet<DownloadJob> DownloadJobs { get; set; }
    public DbSet<Preset> Presets { get; set; }
    public DbSet<AppSettings> AppSettings { get; set; }
    public DbSet<DownloadSettings> DownloadSettings { get; set; }
    public DbSet<FormatSettings> FormatSettings { get; set; }
    public DbSet<EngineSettings> EngineSettings { get; set; }
    public DbSet<AdvancedSettings> AdvancedSettings { get; set; }

    public DownloaderDbContext(DbContextOptions<DownloaderDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        DbContextConfiguration.ConfigureEnumConversions(modelBuilder);

        modelBuilder.Entity<DownloadJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        modelBuilder.Entity<Preset>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.IsBuiltIn).IsRequired();
            entity.HasIndex(e => e.IsBuiltIn);
        });

        modelBuilder.Entity<AppSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Theme).IsRequired();
        });

        modelBuilder.Entity<DownloadSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OutputFolder).IsRequired();
            entity.Property(e => e.FilenameTemplate).IsRequired();
        });

        modelBuilder.Entity<FormatSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<EngineSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<AdvancedSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
