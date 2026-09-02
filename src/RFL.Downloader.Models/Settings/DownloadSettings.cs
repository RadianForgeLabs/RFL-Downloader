namespace RFL.Downloader.Models.Settings;

public class DownloadSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OutputFolder { get; set; } = GetDefaultDownloadsFolder();
    public string FilenameTemplate { get; set; } = "%(title)s.%(ext)s";
    public OverwriteBehavior OverwriteBehavior { get; set; } = OverwriteBehavior.Skip;
    public DuplicateHandling DuplicateHandling { get; set; } = DuplicateHandling.Rename;
    public int MaxConcurrentDownloads { get; set; } = 3;
    public int MaxRetries { get; set; } = 3;
    public bool ResumeIncompleteDownloads { get; set; } = true;
    public bool AutoStartQueue { get; set; } = true;
    public bool IncludeMetadata { get; set; } = true;
    public bool IncludeThumbnail { get; set; } = true;
    public bool IncludeSubtitles { get; set; } = false;
    public string? DefaultSubtitleLanguage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    private static string GetDefaultDownloadsFolder()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads");
    }
}

public enum OverwriteBehavior
{
    Skip,
    Overwrite,
    Rename,
    Error
}

public enum DuplicateHandling
{
    Skip,
    Rename,
    Overwrite,
    Error
}
