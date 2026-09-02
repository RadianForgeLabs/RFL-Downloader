using RFL.Downloader.Models.Downloads;
using Xunit;

namespace RFL.Downloader.DownloadManager.Tests;

public class DownloadManagerTests
{
    [Fact]
    public void DownloadJob_ShouldInitializeWithDefaults()
    {
        var job = new DownloadJob();
        
        Assert.NotEqual(Guid.Empty, job.Id);
        Assert.Equal(DownloadStatus.Waiting, job.Status);
        Assert.Equal(0, job.Progress);
        Assert.Equal(0, job.RetryCount);
        Assert.NotNull(job.CreatedAt);
    }

    [Fact]
    public void DownloadJob_ShouldAllowPropertyUpdates()
    {
        var job = new DownloadJob
        {
            Url = "https://example.com/video",
            Title = "Test Video",
            Status = DownloadStatus.Downloading,
            Progress = 50.0
        };

        Assert.Equal("https://example.com/video", job.Url);
        Assert.Equal("Test Video", job.Title);
        Assert.Equal(DownloadStatus.Downloading, job.Status);
        Assert.Equal(50.0, job.Progress);
    }

    [Theory]
    [InlineData(DownloadStatus.Waiting)]
    [InlineData(DownloadStatus.Analyzing)]
    [InlineData(DownloadStatus.Downloading)]
    [InlineData(DownloadStatus.Completed)]
    [InlineData(DownloadStatus.Failed)]
    [InlineData(DownloadStatus.Cancelled)]
    public void DownloadJob_ShouldAcceptAllStatuses(DownloadStatus status)
    {
        var job = new DownloadJob { Status = status };
        Assert.Equal(status, job.Status);
    }
}
