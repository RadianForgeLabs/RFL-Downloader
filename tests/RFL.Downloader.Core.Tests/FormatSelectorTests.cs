using RFL.Downloader.Core.Formatting;
using RFL.Downloader.Models.Media;
using Xunit;

namespace RFL.Downloader.Core.Tests;

public class FormatSelectorTests
{
    [Fact]
    public void SelectBestFormat_WithEmptyList_ReturnsNull()
    {
        var result = FormatSelector.SelectBestFormat(new List<MediaFormat>());
        Assert.Null(result);
    }

    [Fact]
    public void SelectBestFormat_WithVideoFormats_ReturnsHighestResolution()
    {
        var formats = new List<MediaFormat>
        {
            new() { FormatId = "1", Height = 720, VCodec = "h264" },
            new() { FormatId = "2", Height = 1080, VCodec = "h264" },
            new() { FormatId = "3", Height = 480, VCodec = "h264" }
        };

        var result = FormatSelector.SelectBestFormat(formats);
        Assert.NotNull(result);
        Assert.Equal(1080, result?.Height);
    }

    [Fact]
    public void GetAvailableResolutions_WithVideoFormats_ReturnsUniqueResolutions()
    {
        var formats = new List<MediaFormat>
        {
            new() { Height = 720, VCodec = "h264" },
            new() { Height = 1080, VCodec = "h264" },
            new() { Height = 720, VCodec = "h264" },
            new() { Height = 480, VCodec = "h264" }
        };

        var result = FormatSelector.GetAvailableResolutions(formats);
        Assert.Equal(3, result.Count);
        Assert.Contains("1080p", result);
        Assert.Contains("720p", result);
        Assert.Contains("480p", result);
    }

    [Fact]
    public void GetAvailableContainers_WithFormats_ReturnsUniqueContainers()
    {
        var formats = new List<MediaFormat>
        {
            new() { Ext = "mp4" },
            new() { Ext = "mkv" },
            new() { Ext = "mp4" },
            new() { Ext = "webm" }
        };

        var result = FormatSelector.GetAvailableContainers(formats);
        Assert.Equal(3, result.Count);
        Assert.Contains("MP4", result);
        Assert.Contains("MKV", result);
        Assert.Contains("WEBM", result);
    }

    [Fact]
    public void GetYtDlpFormatString_WithVideoAudioMode_ReturnsCombinedFormat()
    {
        var videoFormat = new MediaFormat { FormatId = "137" };
        var audioFormat = new MediaFormat { FormatId = "140" };

        var result = FormatSelector.GetYtDlpFormatString(videoFormat, audioFormat, DownloadMode.VideoAudio);
        Assert.Equal("137+140", result);
    }

    [Fact]
    public void GetYtDlpFormatString_WithAudioOnlyMode_ReturnsBestAudio()
    {
        var result = FormatSelector.GetYtDlpFormatString(null, null, DownloadMode.AudioOnly);
        Assert.Equal("bestaudio/best", result);
    }
}
