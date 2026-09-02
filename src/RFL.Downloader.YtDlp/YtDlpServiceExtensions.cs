/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.Extensions.DependencyInjection;
using RFL.Downloader.Core.Abstractions;
using RFL.Downloader.Core.Process;
using RFL.Downloader.YtDlp;

namespace RFL.Downloader.YtDlp;

public static class YtDlpServiceExtensions
{
    public static IServiceCollection AddYtDlp(this IServiceCollection services, string? executablePath = null)
    {
        services.AddSingleton<IProcessExecutor, ProcessExecutor>();
        services.AddSingleton<IDownloadEngine>(sp => new YtDlpDownloadEngine(
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<YtDlpDownloadEngine>>(),
            sp.GetRequiredService<IProcessExecutor>(),
            executablePath));

        return services;
    }
}
