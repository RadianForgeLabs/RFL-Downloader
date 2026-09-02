/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.Extensions.DependencyInjection;
using RFL.Downloader.Core.Abstractions;
using RFL.Downloader.Core.Process;
using RFL.Downloader.FFmpeg;

namespace RFL.Downloader.FFmpeg;

public static class FfmpegServiceExtensions
{
    public static IServiceCollection AddFFmpeg(this IServiceCollection services, string? executablePath = null)
    {
        services.AddSingleton<IFfmpegService>(sp => new FfmpegService(
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<FfmpegService>>(),
            sp.GetRequiredService<IProcessExecutor>(),
            executablePath));

        return services;
    }
}
