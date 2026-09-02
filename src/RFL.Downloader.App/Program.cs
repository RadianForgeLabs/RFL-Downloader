/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using WinRT;
using Microsoft.UI.Xaml;

namespace RFL.Downloader.App;

public class Program
{
    [MTAThread]
    static int Main(string[] args)
    {
        WinRT.Compatibility.Initialize();
        Application.Start((p) => new App());
    }
}
