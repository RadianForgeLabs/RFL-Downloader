namespace RFL.Downloader.Models.Settings;

public class AdvancedSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? CustomYtDlpParameters { get; set; }
    public LoggingLevel LoggingLevel { get; set; } = LoggingLevel.Information;
    public bool DebugMode { get; set; }
    public bool EnableDiagnostics { get; set; }
    public int LogRetentionDays { get; set; } = 30;
    public bool EnablePerformanceMetrics { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum LoggingLevel
{
    Verbose,
    Debug,
    Information,
    Warning,
    Error,
    Fatal
}
