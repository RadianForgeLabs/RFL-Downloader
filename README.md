# RFL Downloader

**RFL Studios**  
**RADIANFORGELABS**

RFL Downloader is an AIO downloader application developed by RFL Studios, the software division of RADIANFORGELABS. It is a modern Windows downloader powered by yt-dlp and FFmpeg, built with WinUI 3 for a polished Windows 11 experience.

SPDX-License-Identifier: GPL-3.0-only

## Features

- **Multi-Engine Support**: Built on an abstraction layer that supports multiple download engines
- **yt-dlp Integration**: Full support for yt-dlp's extensive website/media compatibility
- **FFmpeg Processing**: Automatic media merging and format conversion
- **Format Discovery**: Dynamic format detection from real engine output
- **Smart Selection**: Intelligent quality and format selection
- **Download Queue**: Persistent queue management with concurrency control
- **History Tracking**: Complete download history with search and filtering
- **Preset System**: Built-in and custom download presets
- **Modern UI**: Fluent Design with WinUI 3
- **Dark/Light Theme**: System theme support
- **Keyboard Shortcuts**: Full keyboard navigation support

## Requirements

- Windows 10 version 19041 or higher (Windows 11 recommended)
- .NET 10.0 Runtime
- yt-dlp (bundled or custom)
- FFmpeg (bundled or custom)

## Building

### Prerequisites

- .NET 10.0 SDK
- Visual Studio 2022 or later with:
  - .NET desktop development workload
  - Windows App SDK C# templates
  - C++ desktop development (for Windows App SDK)

### Build Steps

1. Clone the repository:
```bash
git clone https://github.com/your-repo/RFL-Downloader.git
cd RFL-Downloader
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build --configuration Release
```

4. Run the application:
```bash
dotnet run --project src/RFL.Downloader.App
```

## Running

### Development

Run from Visual Studio or using:
```bash
dotnet run --project src/RFL.Downloader.App
```

### Production

Build the release version and run the executable from:
```
src\RFL.Downloader.App\bin\Release\net10.0-windows10.0.19041.0\win-x64\
```

## Project Structure

```
RFL-Downloader/
├── src/
│   ├── RFL.Downloader.App/          # WinUI 3 application
│   ├── RFL.Downloader.Core/         # Core abstractions and business logic
│   ├── RFL.Downloader.Models/       # Domain models
│   ├── RFL.Downloader.Infrastructure/# Database, logging, repositories
│   ├── RFL.Downloader.YtDlp/       # yt-dlp engine implementation
│   ├── RFL.Downloader.FFmpeg/      # FFmpeg service implementation
│   ├── RFL.Downloader.DownloadManager/# Queue management and orchestration
│   └── RFL.Downloader.NativeHost/   # Browser integration boundary
├── tests/
│   ├── RFL.Downloader.Core.Tests/
│   ├── RFL.Downloader.YtDlp.Tests/
│   └── RFL.Downloader.DownloadManager.Tests/
├── tools/
│   ├── yt-dlp/                      # Bundled yt-dlp executable
│   └── ffmpeg/                      # Bundled FFmpeg executables
└── docs/
```

## Usage

1. **Analyze URLs**: Paste a URL and click "Analyze" to retrieve media information
2. **Select Format**: Choose from available resolutions and formats
3. **Configure Options**: Set output format, quality, and other preferences
4. **Download**: Add to queue and monitor progress
5. **Manage Queue**: Pause, resume, cancel, and retry downloads
6. **View History**: Access completed downloads and historical records

## Browser Integration

The application includes a native messaging host structure for future browser extension integration. This allows seamless sending of URLs from Chrome/Edge extensions directly to RFL Downloader.

## Configuration

### Settings

- **General**: Theme, startup behavior, notifications
- **Downloads**: Output path, concurrency, filename templates
- **Formats**: Preferred resolutions, containers, codecs
- **Engines**: yt-dlp and FFmpeg paths and update settings
- **Advanced**: Custom parameters, logging, diagnostics

### Presets

Built-in presets include:
- Best Quality
- 1080p MP4
- 720p MP4
- Best Audio MP3
- Best Audio FLAC
- Archive MKV

Custom presets can be created and saved.

## Troubleshooting

### Engine Not Found

If yt-dlp or FFmpeg is not found:
1. Check Settings > Engines for executable paths
2. Use bundled versions from the Tools directory
3. Ensure executables have proper permissions

### Download Failures

- Check network connectivity
- Verify URL is supported by yt-dlp
- Review error messages in the download details
- Check available disk space

### Database Issues

The application uses SQLite for persistence. Database is located at:
```
%LocalAppData%\RFL Downloader\rfl-downloader.db
```

## Third-Party Components

- **yt-dlp**: YouTube video downloader (GPLv3)
- **FFmpeg**: Multimedia framework (GPLv2/LGPLv2.1)
- **WinUI 3**: Windows App SDK (MIT)
- **.NET**: Microsoft .NET (MIT)

See THIRD-PARTY-NOTICES.md for detailed license information.

## Development

See [DEVELOPMENT.md](docs/DEVELOPMENT.md) for detailed development instructions.

## Architecture

See [ARCHITECTURE.md](docs/ARCHITECTURE.md) for detailed architecture documentation.

## License

Copyright (c) 2026 RADIANFORGELABS / RFL Studios

RFL Downloader is licensed under the GNU General Public License v3.0.

See the [LICENSE](LICENSE) file for the complete license text.

This project may include, bundle, invoke, or interact with third-party software that is licensed separately. See [THIRD-PARTY-NOTICES.md](docs/THIRD-PARTY-NOTICES.md) for applicable third-party licensing information.

## Credits

- **Development**: RADIANFORGELABS
- **Design**: RFL Studios
- **yt-dlp**: https://github.com/yt-dlp/yt-dlp
- **FFmpeg**: https://ffmpeg.org/
