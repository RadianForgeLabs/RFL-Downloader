/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using RFL.Downloader.App.Services;

namespace RFL.Downloader.App;

public sealed partial class MainWindow : Window
{
    private readonly NavigationService _navigationService;

    public MainWindow(NavigationService navigationService)
    {
        this.InitializeComponent();
        _navigationService = navigationService;
        _navigationService.SetFrame(ContentFrame);

        MainNavigation.SelectionChanged += MainNavigation_SelectionChanged;
        MainNavigation.SelectedItem = MainNavigation.MenuItems[0];
    }

    private void MainNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            NavigateToPage(tag);
        }
    }

    private void NavigateToPage(string? tag)
    {
        if (string.IsNullOrEmpty(tag))
            return;

        var pageType = tag switch
        {
            "home" => typeof(Pages.HomePage),
            "downloads" => typeof(Pages.DownloadsPage),
            "queue" => typeof(Pages.QueuePage),
            "history" => typeof(Pages.HistoryPage),
            "presets" => typeof(Pages.PresetsPage),
            "settings" => typeof(Pages.SettingsPage),
            "about" => typeof(Pages.AboutPage),
            _ => typeof(Pages.HomePage)
        };

        _navigationService.NavigateTo(pageType);
    }
}
