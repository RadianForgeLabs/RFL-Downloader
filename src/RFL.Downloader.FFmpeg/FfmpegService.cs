/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.Extensions.Logging;
using RFL.Downloader.Core.Abstractions;
using RFL.Downloader.Core.Process;
using RFL.Downloader.Models.Engines;
using RFL.Downloader.Models.Media;

namespace RFL.Downloader.FFmpeg;

public class FfmpegService : IFfmpegService
{
    private readonly ILogger<FfmpegService> _logger;
    private readonly IProcessExecutor _processExecutor;
    private readonly string _executablePath;

    public string Name => "FFmpeg";
    public bool IsAvailable { get; private set; }

    public FfmpegService(
        ILogger<FfmpegService> logger,
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

    public async Task<string?> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
            return null;

        try
        {
            var arguments = new List<string> { "-version" };
            var result = await _processExecutor.ExecuteAsync(_executablePath, arguments, cancellationToken);

            if (result.Success)
            {
                var firstLine = result.StandardOutput.Split('\n').FirstOrDefault();
                return firstLine?.Trim();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get FFmpeg version");
        }

        return null;
    }

    public async Task<bool> CheckForUpdateAsync(CancellationToken cancellationToken = default)
    {
        // FFmpeg updates are typically manual, return false for now
        await Task.CompletedTask;
        return false;
    }

    public async Task MergeStreamsAsync(
        string videoPath,
        string audioPath,
        string outputPath,
        IProgress<ConversionProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException("FFmpeg is not available");
        }

        if (!File.Exists(videoPath))
        {
            throw new FileNotFoundException("Video file not found", videoPath);
        }

        if (!File.Exists(audioPath))
        {
            throw new FileNotFoundException("Audio file not found", audioPath);
        }

        var arguments = new List<string>
        {
            "-i", videoPath,
            "-i", audioPath,
            "-c:v", "copy",
            "-c:a", "aac",
            "-y",
            outputPath
        };

        var progressAdapter = new Progress<ProcessOutput>(output =>
        {
            var conversionProgress = ParseProgress(output.Data);
            if (conversionProgress != null)
            {
                progress?.Report(conversionProgress);
            }
        });

        var result = await _processExecutor.ExecuteAsync(
            _executablePath,
            arguments,
            cancellationToken,
            progressAdapter);

        if (!result.Success)
        {
            throw new InvalidOperationException($"FFmpeg merge failed: {result.StandardError}");
        }
    }

    public async Task ConvertFormatAsync(
        string inputPath,
        string outputPath,
        string? format = null,
        IProgress<ConversionProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException("FFmpeg is not available");
        }

        if (!File.Exists(inputPath))
        {
            throw new FileNotFoundException("Input file not found", inputPath);
        }

        var arguments = new List<string>
        {
            "-i", inputPath,
            "-c", "copy",
            "-y"
        };

        if (!string.IsNullOrWhiteSpace(format))
        {
            arguments.Add("-f");
            arguments.Add(format);
        }

        arguments.Add(outputPath);

        var progressAdapter = new Progress<ProcessOutput>(output =>
        {
            var conversionProgress = ParseProgress(output.Data);
            if (conversionProgress != null)
            {
                progress?.Report(conversionProgress);
            }
        });

        var result = await _processExecutor.ExecuteAsync(
            _executablePath,
            arguments,
            cancellationToken,
            progressAdapter);

        if (!result.Success)
        {
            throw new InvalidOperationException($"FFmpeg conversion failed: {result.StandardError}");
        }
    }

    public async Task<MediaInfo?> GetMediaInfoAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            return null;
        }

        if (!File.Exists(filePath))
        {
            return null;
        }

        var arguments = new List<string>
        {
            "-i", filePath,
            "-hide_banner"
        };

        try
        {
            var result = await _processExecutor.ExecuteAsync(_executablePath, arguments, cancellationToken);
            // Parse FFprobe-style output (basic implementation)
            return new MediaInfo
            {
                Title = Path.GetFileNameWithoutExtension(filePath),
                WebpageUrl = filePath
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task CancelAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling FFmpeg operation");
        await Task.CompletedTask;
    }

    private static string GetDefaultExecutablePath()
    {
        var appPath = AppContext.BaseDirectory;
        var toolsPath = Path.Combine(appPath, "Tools", "ffmpeg", "ffmpeg.exe");

        if (File.Exists(toolsPath))
            return toolsPath;

        var repoToolsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "tools", "ffmpeg", "ffmpeg.exe");
        if (File.Exists(repoToolsPath))
            return Path.GetFullPath(repoToolsPath);

        return "ffmpeg.exe";
    }

    private static ConversionProgress? ParseProgress(string output)
    {
        if (string.IsNullOrWhiteSpace(output))
            return null;

        try
        {
            var progress = new ConversionProgress();

            var timeMatch = System.Text.RegularExpressions.Regex.Match(output, @"time=(\d+:\d+:\d+\.\d+)");
            if (timeMatch.Success)
            {
                progress.StatusMessage = $"Processing: {timeMatch.Groups[1].Value}";
            }

            var sizeMatch = System.Text.RegularExpressions.Regex.Match(output, @"size=(\d+kB)");
            if (sizeMatch.Success)
            {
                progress.StatusMessage += $" Size: {sizeMatch.Groups[1].Value}";
            }

            return progress;
        }
        catch
        {
            return null;
        }
    }
}
