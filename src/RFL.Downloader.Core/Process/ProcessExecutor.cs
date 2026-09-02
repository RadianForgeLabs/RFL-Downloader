using System.Diagnostics;

namespace RFL.Downloader.Core.Process;

public class ProcessExecutor : IProcessExecutor
{
    public async Task<ProcessResult> ExecuteAsync(
        string executable,
        IEnumerable<string> arguments,
        CancellationToken cancellationToken = default,
        IProgress<ProcessOutput>? progress = null)
    {
        return await ExecuteAsync(executable, arguments, null, cancellationToken, progress);
    }

    public async Task<ProcessResult> ExecuteAsync(
        string executable,
        IEnumerable<string> arguments,
        string? workingDirectory = null,
        CancellationToken cancellationToken = default,
        IProgress<ProcessOutput>? progress = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            StandardErrorEncoding = System.Text.Encoding.UTF8
        };

        foreach (var arg in arguments)
        {
            startInfo.ArgumentList.Add(arg);
        }

        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var outputBuilder = new System.Text.StringBuilder();
        var errorBuilder = new System.Text.StringBuilder();

        using var process = new System.Diagnostics.Process { StartInfo = startInfo };

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                outputBuilder.AppendLine(e.Data);
                progress?.Report(new ProcessOutput
                {
                    Data = e.Data,
                    IsError = false
                });
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                errorBuilder.AppendLine(e.Data);
                progress?.Report(new ProcessOutput
                {
                    Data = e.Data,
                    IsError = true
                });
            }
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch
            {
                // Ignore errors during cleanup
            }
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }

        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            StandardOutput = outputBuilder.ToString(),
            StandardError = errorBuilder.ToString(),
            ExecutionTime = stopwatch.Elapsed
        };
    }
}
