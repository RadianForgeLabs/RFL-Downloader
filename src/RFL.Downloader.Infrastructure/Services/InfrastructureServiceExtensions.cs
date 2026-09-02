using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RFL.Downloader.Infrastructure.Data;
using RFL.Downloader.Infrastructure.Logging;
using RFL.Downloader.Infrastructure.Repositories;
using Serilog;

namespace RFL.Downloader.Infrastructure.Services;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string? databasePath = null,
        string? logFilePath = null)
    {
        LoggingService.ConfigureLogging(logFilePath);

        var dbPath = databasePath ?? GetDefaultDatabasePath();

        services.AddDbContext<DownloaderDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });

        services.AddScoped<DownloadJobRepository>();
        services.AddScoped<PresetRepository>();
        services.AddScoped<SettingsRepository>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DownloaderDbContext>();

        await context.Database.EnsureCreatedAsync();

        var presetRepository = scope.ServiceProvider.GetRequiredService<PresetRepository>();
        await presetRepository.EnsureBuiltInPresetsAsync();
    }

    private static string GetDefaultDatabasePath()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RFL Downloader");

        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }

        return Path.Combine(appDataPath, "rfl-downloader.db");
    }
}
