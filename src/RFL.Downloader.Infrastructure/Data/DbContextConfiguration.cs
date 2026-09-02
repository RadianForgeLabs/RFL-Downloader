using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RFL.Downloader.Models.Downloads;
using RFL.Downloader.Models.Media;
using RFL.Downloader.Models.Settings;

namespace RFL.Downloader.Infrastructure.Data;

public static class DbContextConfiguration
{
    public static void ConfigureEnumConversions(ModelBuilder modelBuilder)
    {
        var downloadStatusConverter = new EnumToNumberConverter<DownloadStatus, int>();
        var downloadModeConverter = new EnumToNumberConverter<DownloadMode, int>();
        var outputFormatConverter = new EnumToNumberConverter<OutputFormat, int>();
        var subtitleFormatConverter = new EnumToNumberConverter<SubtitleFormat, int>();
        var applicationThemeConverter = new EnumToNumberConverter<ApplicationTheme, int>();
        var overwriteBehaviorConverter = new EnumToNumberConverter<OverwriteBehavior, int>();
        var duplicateHandlingConverter = new EnumToNumberConverter<DuplicateHandling, int>();
        var loggingLevelConverter = new EnumToNumberConverter<LoggingLevel, int>();

        modelBuilder.Entity<DownloadJob>()
            .Property(e => e.Status)
            .HasConversion(downloadStatusConverter);

        modelBuilder.Entity<DownloadJob>()
            .Property(e => e.Mode)
            .HasConversion(downloadModeConverter);

        modelBuilder.Entity<DownloadJob>()
            .Property(e => e.OutputFormat)
            .HasConversion(outputFormatConverter);

        modelBuilder.Entity<DownloadJob>()
            .Property(e => e.SubtitleFormat)
            .HasConversion(subtitleFormatConverter);

        modelBuilder.Entity<AppSettings>()
            .Property(e => e.Theme)
            .HasConversion(applicationThemeConverter);

        modelBuilder.Entity<DownloadSettings>()
            .Property(e => e.OverwriteBehavior)
            .HasConversion(overwriteBehaviorConverter);

        modelBuilder.Entity<DownloadSettings>()
            .Property(e => e.DuplicateHandling)
            .HasConversion(duplicateHandlingConverter);

        modelBuilder.Entity<AdvancedSettings>()
            .Property(e => e.LoggingLevel)
            .HasConversion(loggingLevelConverter);
    }
}
