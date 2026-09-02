using RFL.Downloader.Models.Media;

namespace RFL.Downloader.Models.Presets;

public class Preset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsBuiltIn { get; set; }
    public DownloadMode Mode { get; set; } = DownloadMode.VideoAudio;
    public string? ResolutionPreference { get; set; }
    public string? VideoCodec { get; set; }
    public string? AudioCodec { get; set; }
    public OutputFormat? PreferredContainer { get; set; }
    public string? Quality { get; set; }
    public bool IncludeSubtitles { get; set; }
    public string? SubtitleLanguage { get; set; }
    public SubtitleFormat? SubtitleFormat { get; set; }
    public bool IncludeMetadata { get; set; }
    public bool IncludeThumbnail { get; set; }
    public string? FilenameTemplate { get; set; }
    public string? OutputFolder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int SortOrder { get; set; }
}
