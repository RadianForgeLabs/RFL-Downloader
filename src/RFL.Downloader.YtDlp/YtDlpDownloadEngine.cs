using Microsoft.Extensions.Logging;
using RFL.Downloader.Core.Abstractions;
using RFL.Downloader.Core.Errors;
using RFL.Downloader.Core.Process;
using RFL.Downloader.Core.Validation;
using RFL.Downloader.Models.Downloads;
using RFL.Downloader.Models.Engines;
using RFL.Downloader.Models.Media;

namespace RFL.Downloader.YtDlp;

public class YtDlpDownloadEngine : IDownloadEngine
{
    private readonly ILogger<YtDlpDownloadEngine> _logger;
    private readonly IProcessExecutor _processExecutor;
    private readonly string _executablePath;

    public string Name => "yt-dlp";
    public string Version { get; private set; } = "Unknown";
    public bool IsAvailable { get; private set; }

    public YtDlpDownloadEngine(
        ILogger<YtDlpDownloadEngine> logger,
        IProcessExecutor processExecutor,
        string? executablePath = null)
    {
        _logger = logger;
        _processExecutor = processExecutor;
        _executablePath = executablePath ?? GetDefaultExecutablePath();
        IsAvailable = File.Exists(_executablePath);
    }

    public async Task<EngineInfo> GetEngineInfoAsync(CancellationToken cancellationToken = default)
    {
        var version = await GetVersionAsync(cancellationToken);
        return new EngineInfo
        {
            Name = Name,
            Version = version ?? "Unknown",
            ExecutablePath = _executablePath,
            IsAvailable = IsAvailable,
            IsBundled = _executablePath.Contains("Tools")
        };
    }

    public async Task<MediaInfo> AnalyzeAsync(string url, CancellationToken cancellationToken = default)
    {
        if (!UrlValidator.IsValidUrl(url))
        {
            throw new ArgumentException("Invalid URL", nameof(url));
        }

        if (!IsAvailable)
        {
            throw new InvalidOperationException("yt-dlp is not available");
        }

        var arguments = new List<string>
        {
            "--dump-json",
            "--no-playlist",
            url
        };

        var result = await _processExecutor.ExecuteAsync(_executablePath, arguments, cancellationToken);

        if (!result.Success)
        {
            var error = ErrorClassifier.ClassifyFromEngineOutput(result.StandardError);
            throw new InvalidOperationException(error.Message, new Exception(error.TechnicalDetails));
        }

        return ParseMediaInfo(result.StandardOutput, url);
    }

    public async Task<IReadOnlyList<MediaFormat>> GetFormatsAsync(string url, CancellationToken cancellationToken = default)
    {
        if (!UrlValidator.IsValidUrl(url))
        {
            throw new ArgumentException("Invalid URL", nameof(url));
        }

        if (!IsAvailable)
        {
            throw new InvalidOperationException("yt-dlp is not available");
        }

        var arguments = new List<string>
        {
            "--list-formats",
            "--print-json",
            url
        };

        var result = await _processExecutor.ExecuteAsync(_executablePath, arguments, cancellationToken);

        if (!result.Success)
        {
            var error = ErrorClassifier.ClassifyFromEngineOutput(result.StandardError);
            throw new InvalidOperationException(error.Message, new Exception(error.TechnicalDetails));
        }

        return ParseFormats(result.StandardOutput);
    }

    public async Task<DownloadJob> DownloadAsync(
        DownloadJob job,
        IProgress<DownloadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException("yt-dlp is not available");
        }

        job.Status = DownloadStatus.Downloading;
        job.StartedAt = DateTime.UtcNow;
        job.Engine = Name;

        var arguments = BuildDownloadArguments(job);

        var progressAdapter = new Progress<ProcessOutput>(output =>
        {
            var downloadProgress = ParseProgress(output.Data);
            if (downloadProgress != null)
            {
                progress?.Report(downloadProgress);
            }
        });

        var result = await _processExecutor.ExecuteAsync(
            _executablePath,
            arguments,
            job.OutputFolder,
            cancellationToken,
            progressAdapter);

        if (!result.Success)
        {
            job.Status = DownloadStatus.Failed;
            job.ErrorMessage = result.StandardError;
            job.CompletedAt = DateTime.UtcNow;
            return job;
        }

        job.Status = DownloadStatus.Completed;
        job.Progress = 100;
        job.CompletedAt = DateTime.UtcNow;

        return job;
    }

    public async Task CancelAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling download job {JobId}", jobId);
        await Task.CompletedTask;
    }

    public async Task<bool> CheckForUpdateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var arguments = new List<string> { "--update-check" };
            var result = await _processExecutor.ExecuteAsync(_executablePath, arguments, cancellationToken);
            return result.Success && result.StandardOutput.Contains("up-to-date", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
            return null;

        try
        {
            var arguments = new List<string> { "--version" };
            var result = await _processExecutor.ExecuteAsync(_executablePath, arguments, cancellationToken);

            if (result.Success)
            {
                Version = result.StandardOutput.Trim();
                return Version;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get yt-dlp version");
        }

        return null;
    }

    private static string GetDefaultExecutablePath()
    {
        var appPath = AppContext.BaseDirectory;
        var toolsPath = Path.Combine(appPath, "Tools", "yt-dlp", "yt-dlp.exe");

        if (File.Exists(toolsPath))
            return toolsPath;

        var repoToolsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "tools", "yt-dlp", "yt-dlp.exe");
        if (File.Exists(repoToolsPath))
            return Path.GetFullPath(repoToolsPath);

        return "yt-dlp.exe";
    }

    private static List<string> BuildDownloadArguments(DownloadJob job)
    {
        var arguments = new List<string>
        {
            "--newline",
            "--progress",
            "--no-playlist"
        };

        if (!string.IsNullOrWhiteSpace(job.OutputFolder))
        {
            arguments.Add("-o");
            arguments.Add(Path.Combine(job.OutputFolder, "%(title)s.%(ext)s"));
        }

        if (!string.IsNullOrWhiteSpace(job.SelectedFormatId))
        {
            arguments.Add("-f");
            arguments.Add(job.SelectedFormatId);
        }

        if (!string.IsNullOrWhiteSpace(job.CustomArguments))
        {
            arguments.AddRange(ParseCustomArguments(job.CustomArguments));
        }

        arguments.Add(job.Url);

        return arguments;
    }

    private static List<string> ParseCustomArguments(string customArgs)
    {
        var args = new List<string>();
        var parts = customArgs.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            if (!string.IsNullOrWhiteSpace(part))
            {
                args.Add(part);
            }
        }

        return args;
    }

    private static MediaInfo ParseMediaInfo(string json, string originalUrl)
    {
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(json);
            var root = document.RootElement;

            var mediaInfo = new MediaInfo
            {
                Id = root.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? string.Empty : string.Empty,
                Title = root.TryGetProperty("title", out var titleProp) ? titleProp.GetString() ?? string.Empty : string.Empty,
                Description = root.TryGetProperty("description", out var descProp) ? descProp.GetString() ?? string.Empty : string.Empty,
                Uploader = root.TryGetProperty("uploader", out var uploaderProp) ? uploaderProp.GetString() ?? string.Empty : string.Empty,
                UploaderId = root.TryGetProperty("uploader_id", out var uploaderIdProp) ? uploaderIdProp.GetString() ?? string.Empty : string.Empty,
                UploaderUrl = root.TryGetProperty("uploader_url", out var uploaderUrlProp) ? uploaderUrlProp.GetString() ?? string.Empty : string.Empty,
                WebpageUrl = root.TryGetProperty("webpage_url", out var webpageUrlProp) ? webpageUrlProp.GetString() ?? string.Empty : string.Empty,
                OriginalUrl = originalUrl,
                ThumbnailUrl = root.TryGetProperty("thumbnail", out var thumbnailProp) ? thumbnailProp.GetString() : null
            };

            if (root.TryGetProperty("duration", out var durationProp) && durationProp.TryGetInt64(out var duration))
            {
                mediaInfo.Duration = TimeSpan.FromSeconds(duration);
            }

            if (root.TryGetProperty("upload_date", out var uploadDateProp) && uploadDateProp.ValueKind != System.Text.Json.JsonValueKind.Null)
            {
                var dateStr = uploadDateProp.GetString();
                if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date))
                {
                    mediaInfo.UploadDate = date;
                }
            }

            if (root.TryGetProperty("view_count", out var viewCountProp) && viewCountProp.TryGetInt64(out var viewCount))
            {
                mediaInfo.ViewCount = viewCount.ToString("N0");
            }

            if (root.TryGetProperty("formats", out var formatsProp))
            {
                foreach (var format in formatsProp.EnumerateArray())
                {
                    mediaInfo.Formats.Add(ParseFormat(format));
                }
            }

            return mediaInfo;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to parse media info", ex);
        }
    }

    private static List<MediaFormat> ParseFormats(string json)
    {
        var formats = new List<MediaFormat>();

        try
        {
            var lines = json.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.TrimStart().StartsWith("{"))
                {
                    try
                    {
                        using var document = System.Text.Json.JsonDocument.Parse(line);
                        formats.Add(ParseFormat(document.RootElement));
                    }
                    catch
                    {
                        // Skip invalid JSON lines
                    }
                }
            }
        }
        catch
        {
            // Return empty list on parse error
        }

        return formats;
    }

    private static MediaFormat ParseFormat(System.Text.Json.JsonElement element)
    {
        var format = new MediaFormat
        {
            FormatId = element.TryGetProperty("format_id", out var formatIdProp) ? formatIdProp.GetString() ?? string.Empty : string.Empty,
            Ext = element.TryGetProperty("ext", out var extProp) ? extProp.GetString() : null,
            Note = element.TryGetProperty("format_note", out var noteProp) ? noteProp.GetString() : null,
            Width = element.TryGetProperty("width", out var widthProp) && widthProp.TryGetInt32(out var width) ? width : null,
            Height = element.TryGetProperty("height", out var heightProp) && heightProp.TryGetInt32(out var height) ? height : null,
            Fps = element.TryGetProperty("fps", out var fpsProp) && fpsProp.TryGetSingle(out var fps) ? fps : null,
            VCodec = element.TryGetProperty("vcodec", out var vcodecProp) ? vcodecProp.GetString() : null,
            ACodec = element.TryGetProperty("acodec", out var acodecProp) ? acodecProp.GetString() : null,
            VExt = element.TryGetProperty("v_ext", out var vextProp) ? vextProp.GetString() : null,
            AExt = element.TryGetProperty("a_ext", out var aextProp) ? aextProp.GetString() : null,
            AB = element.TryGetProperty("abr", out var abProp) ? abProp.GetString() : null,
            Tbr = element.TryGetProperty("tbr", out var tbrProp) ? tbrProp.GetString() : null,
            Vbr = element.TryGetProperty("vbr", out var vbrProp) ? vbrProp.GetString() : null,
            Filesize = element.TryGetProperty("filesize", out var filesizeProp) ? filesizeProp.ToString() : null,
            FormatNote = element.TryGetProperty("format_note", out var formatNoteProp) ? formatNoteProp.GetString() : null,
            Format = element.TryGetProperty("format", out var formatProp) ? formatProp.GetString() : null,
            Quality = element.TryGetProperty("quality", out var qualityProp) ? qualityProp.GetString() : null
        };

        format.HasVideo = format.VCodec != null && format.VCodec != "none";
        format.HasAudio = format.ACodec != null && format.ACodec != "none";
        format.OnlyAudio = format.HasAudio == true && format.HasVideo == false;
        format.OnlyVideo = format.HasVideo == true && format.HasAudio == false;
        format.VideoAndAudio = format.HasVideo == true && format.HasAudio == true;

        return format;
    }

    private static DownloadProgress? ParseProgress(string output)
    {
        if (string.IsNullOrWhiteSpace(output))
            return null;

        try
        {
            if (output.Contains("[download]"))
            {
                var progress = new DownloadProgress();

                var percentageMatch = System.Text.RegularExpressions.Regex.Match(output, @"(\d+\.?\d*)%");
                if (percentageMatch.Success && double.TryParse(percentageMatch.Groups[1].Value, out var percentage))
                {
                    progress.Percentage = percentage;
                }

                var sizeMatch = System.Text.RegularExpressions.Regex.Match(output, @"(\d+\.?\d*[A-Z]+) of (\d+\.?\d*[A-Z]+)");
                if (sizeMatch.Success)
                {
                    progress.StatusMessage = sizeMatch.Value;
                }

                var speedMatch = System.Text.RegularExpressions.Regex.Match(output, @"at (\d+\.?\d*[A-Z]+/s)");
                if (speedMatch.Success)
                {
                    progress.StatusMessage += $" {speedMatch.Value}";
                }

                var etaMatch = System.Text.RegularExpressions.Regex.Match(output, @"ETA (\d+:\d+)");
                if (etaMatch.Success && TimeSpan.TryParse(etaMatch.Groups[1].Value, out var eta))
                {
                    progress.Eta = eta;
                }

                return progress;
            }
        }
        catch
        {
            // Return null on parse error
        }

        return null;
    }
}
