# RFL Downloader Architecture

## Overview

RFL Downloader is built with a clean, layered architecture that separates concerns and enables extensibility. The design supports multiple download engines through abstraction layers.

## Project Structure

### Core Projects

#### RFL.Downloader.Models
Contains all domain models that are shared across the application:
- `MediaInfo`: Information about analyzed media
- `MediaFormat`: Format details from download engines
- `DownloadJob`: Download task representation
- `Preset`: Download configuration presets
- Settings models (AppSettings, DownloadSettings, etc.)

#### RFL.Downloader.Core
Contains engine-agnostic business logic and abstractions:
- `IDownloadEngine`: Abstraction for download engines
- `IFfmpegService`: Abstraction for FFmpeg operations
- `IFormatAnalyzer`: Format analysis and selection
- `IDownloadService`: Download job management
- `IBrowserIntegrationService`: Browser integration boundary
- Utilities: URL validation, filename sanitization, error classification, process execution

#### RFL.Downloader.Infrastructure
Provides persistence and infrastructure services:
- `DownloaderDbContext`: Entity Framework Core database context
- Repositories: DownloadJobRepository, PresetRepository, SettingsRepository
- Logging: Serilog configuration and file logging
- Service extensions for dependency injection

#### RFL.Downloader.YtDlp
Implements the yt-dlp download engine:
- `YtDlpDownloadEngine`: Concrete implementation of IDownloadEngine
- JSON parsing for yt-dlp output
- Progress parsing and error handling
- Safe process execution

#### RFL.Downloader.FFmpeg
Implements FFmpeg integration:
- `FfmpegService`: Concrete implementation of IFfmpegService
- Stream merging
- Format conversion
- Version detection

#### RFL.Downloader.DownloadManager
Orchestrates download operations:
- `DownloadManagerService`: Queue management, concurrency control
- Job lifecycle management
- Progress tracking
- Retry logic
- Cancellation handling

#### RFL.Downloader.App
WinUI 3 application:
- `MainWindow`: Main application window with navigation
- Pages: Home, Downloads, Queue, History, Presets, Settings, About
- ViewModels: MVVM pattern implementation
- Dependency injection setup

#### RFL.Downloader.NativeHost
Native messaging host for browser integration:
- JSON request/response handling
- Communication boundary for future browser extensions

## Key Architectural Patterns

### Download Engine Abstraction

The `IDownloadEngine` interface defines the contract for all download engines:

```csharp
public interface IDownloadEngine
{
    string Name { get; }
    string Version { get; }
    bool IsAvailable { get; }

    Task<EngineInfo> GetEngineInfoAsync(CancellationToken cancellationToken = default);
    Task<MediaInfo> AnalyzeAsync(string url, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MediaFormat>> GetFormatsAsync(string url, CancellationToken cancellationToken = default);
    Task<DownloadJob> DownloadAsync(DownloadJob job, IProgress<DownloadProgress>? progress = null, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<bool> CheckForUpdateAsync(CancellationToken cancellationToken = default);
    Task<string?> GetVersionAsync(CancellationToken cancellationToken = default);
}
```

This allows future engines (e.g., DirectHttpDownloadEngine) to be added without modifying the UI.

### Process Execution

Safe process execution is handled through `IProcessExecutor`:

```csharp
public interface IProcessExecutor
{
    Task<ProcessResult> ExecuteAsync(
        string executable,
        IEnumerable<string> arguments,
        CancellationToken cancellationToken = default,
        IProgress<ProcessOutput>? progress = null);
}
```

This prevents shell injection and provides structured output parsing.

### Database Design

SQLite with Entity Framework Core is used for persistence:

- **DownloadJobs**: Complete download history and queue
- **Presets**: User-defined and built-in presets
- **Settings**: Application configuration
- Enum values are stored as integers using value converters

### Dependency Injection

Microsoft.Extensions.DependencyInjection is used throughout:

```csharp
services.AddInfrastructure();
services.AddYtDlp();
services.AddFFmpeg();
services.AddSingleton<DownloadManagerService>();
services.AddSingleton<NavigationService>();
```

## Data Flow

### URL Analysis Flow

1. User enters URL in Home page
2. URL is validated using `UrlValidator`
3. URL is passed to `IDownloadEngine.AnalyzeAsync()`
4. Engine returns `MediaInfo` with formats
5. Formats are displayed in UI
6. User selects format and options
7. Download job is created and queued

### Download Flow

1. `DownloadManagerService.EnqueueDownloadAsync()` adds job to queue
2. Queue processor picks up job based on concurrency limits
3. `IDownloadEngine.DownloadAsync()` executes download
4. Progress is reported via `IProgress<DownloadProgress>`
5. If needed, `IFfmpegService` performs merging/conversion
6. Job status is updated in database
7. Completion is recorded in history

## Security Considerations

### Input Validation
- All URLs are validated before processing
- Filenames are sanitized to prevent path traversal
- Process arguments are safely constructed (no shell injection)

### Process Safety
- Direct process execution without shell
- Argument lists instead of command strings
- Proper cancellation token handling
- Process cleanup on errors

### Data Protection
- No passwords stored in database
- Sensitive URL parameters redacted from logs
- Local-only storage by default

## Extensibility Points

### Adding New Download Engines

1. Implement `IDownloadEngine`
2. Add service registration:
```csharp
services.AddSingleton<IDownloadEngine, NewEngine>();
```
3. UI automatically supports the new engine

### Adding New Presets

1. Create `Preset` object
2. Use `PresetRepository.AddAsync()`
3. Preset appears in UI automatically

### Browser Integration

The `IBrowserIntegrationService` provides a clean boundary for future browser extension integration. The native host structure is already in place in `RFL.Downloader.NativeHost`.

## Performance Considerations

- Async/await throughout to prevent UI blocking
- Cancellation tokens for responsive cancellation
- Connection pooling for database access
- Efficient progress reporting (throttled database updates)
- Concurrent download limits to prevent resource exhaustion

## Future Enhancements

The architecture supports:
- Additional download engines (HTTP, FTP, etc.)
- Plugin system for custom processors
- Cloud storage integration
- Network optimization
- Advanced scheduling
- Multi-language support
