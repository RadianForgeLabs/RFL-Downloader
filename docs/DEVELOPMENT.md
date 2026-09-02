# Development Guide

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- Visual Studio 2022 or later with:
  - .NET desktop development workload
  - Windows App SDK C# templates
  - C++ desktop development (for Windows App SDK)
- Git

### Setting Up the Development Environment

1. Clone the repository:
```bash
git clone https://github.com/your-repo/RFL-Downloader.git
cd RFL-Downloader
```

2. Open the solution in Visual Studio:
```
RFL.Downloader.sln
```

3. Restore NuGet packages:
```bash
dotnet restore
```

4. Build the solution:
```bash
dotnet build
```

## Project Structure

### Source Projects

- **RFL.Downloader.App**: WinUI 3 application shell and UI
- **RFL.Downloader.Core**: Core abstractions and business logic
- **RFL.Downloader.Models**: Domain models
- **RFL.Downloader.Infrastructure**: Database, logging, repositories
- **RFL.Downloader.YtDlp**: yt-dlp engine implementation
- **RFL.Downloader.FFmpeg**: FFmpeg service implementation
- **RFL.Downloader.DownloadManager**: Queue management
- **RFL.Downloader.NativeHost**: Browser integration boundary

### Test Projects

- **RFL.Downloader.Core.Tests**: Core functionality tests
- **RFL.Downloader.YtDlp.Tests**: yt-dlp integration tests
- **RFL.Downloader.DownloadManager.Tests**: Download manager tests

## Building

### Debug Build

```bash
dotnet build --configuration Debug
```

### Release Build

```bash
dotnet build --configuration Release
```

### Build for Specific Platform

```bash
dotnet build --configuration Release -p:Platform=x64
```

## Running

### From Visual Studio

Set `RFL.Downloader.App` as the startup project and press F5.

### From Command Line

```bash
dotnet run --project src/RFL.Downloader.App
```

## Testing

### Run All Tests

```bash
dotnet test
```

### Run Specific Test Project

```bash
dotnet test tests/RFL.Downloader.Core.Tests
```

### Run with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Debugging

### Debugging WinUI 3 Applications

1. Set breakpoints in your code
2. Set RFL.Downloader.App as startup project
3. Press F5 or click "Start Debugging"

### Debugging Download Engines

The download engines run in background processes. To debug:
1. Attach to process after download starts
2. Use logging to trace execution
3. Add debug output in engine implementation

## Database Management

### Database Location

```
%LocalAppData%\RFL Downloader\rfl-downloader.db
```

### Resetting Database

Delete the database file and restart the application. The database will be recreated with default settings and built-in presets.

### Database Migrations

Currently using `EnsureCreatedAsync()` for simplicity. For production, consider using EF Core migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Bundled Tools

### yt-dlp

Place yt-dlp executable in:
```
tools/yt-dlp/yt-dlp.exe
```

Or configure custom path in Settings > Engines.

### FFmpeg

Place FFmpeg executables in:
```
tools/ffmpeg/ffmpeg.exe
tools/ffmpeg/ffprobe.exe
```

Or configure custom path in Settings > Engines.

## Logging

### Log Location

```
%LocalAppData%\RFL Downloader\Logs\rfl-downloader-<date>.log
```

### Log Levels

Configure in Settings > Advanced > Logging Level:
- Verbose
- Debug
- Information (default)
- Warning
- Error
- Fatal

## Code Style

### Naming Conventions

- **Classes**: PascalCase (`DownloadManager`)
- **Methods**: PascalCase (`AnalyzeAsync`)
- **Properties**: PascalCase (`MediaInfo`)
- **Local variables**: camelCase (`mediaInfo`)
- **Constants**: PascalCase (`MaxConcurrentDownloads`)

### File Organization

- One class per file
- File name matches class name
- Organize by namespace folders
- Use `#region` for large files

### Async/Await

- Use `async`/`await` for I/O operations
- Pass `CancellationToken` where appropriate
- Configure `await` using `ConfigureAwait(false)` in library code

### Error Handling

- Use specific exception types
- Include meaningful error messages
- Log errors with context
- Never expose stack traces to users

## Adding Features

### Adding a New Page

1. Create XAML file in `Pages/` folder
2. Create code-behind file
3. Register in `MainWindow.xaml.cs` navigation
4. Add NavigationViewItem in `MainWindow.xaml`

### Adding a New Setting

1. Add property to appropriate settings model
2. Update database context if needed
3. Add UI control in Settings page
4. Wire up save/load logic

### Adding a New Download Engine

1. Implement `IDownloadEngine`
2. Create service registration extension
3. Register in `App.xaml.cs`
4. Update Settings > Engines to display engine info

## Performance Optimization

### Database

- Use indexing for frequently queried fields
- Consider caching for read-heavy operations
- Batch updates where possible

### UI

- Use virtualization for large lists
- Throttle progress updates
- Debounce search/filter operations

### Downloads

- Limit concurrent downloads
- Implement proper cancellation
- Clean up resources promptly

## Deployment

### Building Installer

The project structure supports future installer creation. Consider:
- WiX Toolset for MSI
- Inno Setup for exe installer
- MSIX for Windows Store distribution

### Configuration Files

Ensure the following are included in deployment:
- `app.manifest`
- Application icons
- Bundled tools (yt-dlp, FFmpeg)
- Third-party licenses

## Troubleshooting

### Build Errors

**Missing Windows App SDK**: Install via Visual Studio Installer

**Missing workloads**: Install ".NET desktop development" and "C++ desktop development"

**Target framework not found**: Install .NET 10.0 SDK

### Runtime Errors

**Engine not found**: Check Settings > Engines for executable paths

**Database locked**: Ensure only one instance is running

**Permission denied**: Run as administrator or check file permissions

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## Resources

- [WinUI 3 Documentation](https://docs.microsoft.com/windows/apps/winui/winui3/)
- [Windows App SDK](https://docs.microsoft.com/windows/apps/windows-app-sdk/)
- [yt-dlp Documentation](https://github.com/yt-dlp/yt-dlp)
- [FFmpeg Documentation](https://ffmpeg.org/documentation.html)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
