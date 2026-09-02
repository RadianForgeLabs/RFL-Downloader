namespace RFL.Downloader.Models.Settings;

public class EngineSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? YtDlpPath { get; set; }
    public string? YtDlpVersion { get; set; }
    public bool UseBundledYtDlp { get; set; } = true;
    public bool AutoUpdateYtDlp { get; set; } = true;
    public DateTime? LastYtDlpUpdateCheck { get; set; }
    public string? FFmpegPath { get; set; }
    public string? FFmpegVersion { get; set; }
    public bool UseBundledFFmpeg { get; set; } = true;
    public bool AutoUpdateFFmpeg { get; set; } = false;
    public DateTime? LastFFmpegUpdateCheck { get; set; }
    public string? CustomYtDlpArguments { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
