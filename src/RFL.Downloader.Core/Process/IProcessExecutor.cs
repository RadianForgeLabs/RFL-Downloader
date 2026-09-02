/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace RFL.Downloader.Core.Process;

public interface IProcessExecutor
{
    Task<ProcessResult> ExecuteAsync(
        string executable,
        IEnumerable<string> arguments,
        CancellationToken cancellationToken = default,
        IProgress<ProcessOutput>? progress = null);

    Task<ProcessResult> ExecuteAsync(
        string executable,
        IEnumerable<string> arguments,
        string? workingDirectory = null,
        CancellationToken cancellationToken = default,
        IProgress<ProcessOutput>? progress = null);
}

public class ProcessResult
{
    public int ExitCode { get; set; }
    public string StandardOutput { get; set; } = string.Empty;
    public string StandardError { get; set; } = string.Empty;
    public bool Success => ExitCode == 0;
    public TimeSpan ExecutionTime { get; set; }
}

public class ProcessOutput
{
    public string Data { get; set; } = string.Empty;
    public bool IsError { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
