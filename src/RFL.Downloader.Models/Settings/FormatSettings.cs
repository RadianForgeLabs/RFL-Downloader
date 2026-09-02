using RFL.Downloader.Models.Media;

namespace RFL.Downloader.Models.Settings;

public class FormatSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? PreferredResolution { get; set; }
    public OutputFormat? PreferredContainer { get; set; } = OutputFormat.Mp4;
    public string? PreferredVideoCodec { get; set; }
    public string? PreferredAudioCodec { get; set; }
    public DownloadMode DefaultMode { get; set; } = DownloadMode.VideoAudio;
    public bool PreferHdr { get; set; }
    public bool PreferHighFps { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
