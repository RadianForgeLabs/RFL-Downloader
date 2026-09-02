/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using RFL.Downloader.App.Services;
using RFL.Downloader.DownloadManager;
using RFL.Downloader.Infrastructure.Services;
using RFL.Downloader.Infrastructure.Logging;
using RFL.Downloader.YtDlp;
using RFL.Downloader.FFmpeg;
using Serilog;
using Serilog.Extensions.Hosting;
using WinRT;

namespace RFL.Downloader.App;

public partial class App : Application
{
    private IHost? _host;

    public App()
    {
        InitializeComponent();
        UnhandledException += App_UnhandledException;
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        _host = Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                services.AddInfrastructure();
                services.AddYtDlp();
                services.AddFFmpeg();
                services.AddSingleton<DownloadManagerService>();
                services.AddSingleton<NavigationService>();
                services.AddTransient<MainWindow>();
            })
            .Build();

        await InfrastructureServiceExtensions.InitializeDatabaseAsync(_host.Services);

        var navigationService = _host.Services.GetRequiredService<NavigationService>();
        var window = new MainWindow(navigationService);
        window.Activate();
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Log.Error(e.Exception, "Unhandled exception");
        e.Handled = true;
    }
}
