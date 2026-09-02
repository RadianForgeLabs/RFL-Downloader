/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.UI.Xaml;

namespace RFL.Downloader.App;

public class Program
{
    [MTAThread]
    static int Main(string[] args)
    {
        Application.Start((p) => new App());
        return 0;
    }
}
