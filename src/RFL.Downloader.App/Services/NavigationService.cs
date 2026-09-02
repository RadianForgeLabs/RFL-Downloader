/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace RFL.Downloader.App.Services;

public class NavigationService
{
    private Frame? _frame;

    public void SetFrame(Frame frame)
    {
        _frame = frame;
    }

    public void NavigateTo(Type pageType, object? parameter = null)
    {
        if (_frame != null)
        {
            _frame.Navigate(pageType, parameter);
        }
    }

    public bool GoBack()
    {
        if (_frame != null && _frame.CanGoBack)
        {
            _frame.GoBack();
            return true;
        }
        return false;
    }

    public bool CanGoBack()
    {
        return _frame?.CanGoBack ?? false;
    }
}
