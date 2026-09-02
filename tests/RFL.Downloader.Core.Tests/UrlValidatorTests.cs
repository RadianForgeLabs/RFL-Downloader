using RFL.Downloader.Core.Validation;
using Xunit;

namespace RFL.Downloader.Core.Tests;

public class UrlValidatorTests
{
    [Theory]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ", true)]
    [InlineData("http://example.com/video.mp4", true)]
    [InlineData("https://vimeo.com/123456789", true)]
    [InlineData("not-a-url", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("ftp://files.example.com/video.mkv", true)]
    public void IsValidUrl_WithVariousInputs_ReturnsExpectedResult(string? url, bool expected)
    {
        var result = UrlValidator.IsValidUrl(url);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ", true)]
    [InlineData("https://youtu.be/dQw4w9WgXcQ", true)]
    [InlineData("http://youtube.com/watch?v=dQw4w9WgXcQ", true)]
    [InlineData("https://vimeo.com/123456789", false)]
    [InlineData("not-a-url", false)]
    public void IsYoutubeUrl_WithVariousInputs_ReturnsExpectedResult(string? url, bool expected)
    {
        var result = UrlValidator.IsYoutubeUrl(url);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.com/video.mp4", true)]
    [InlineData("http://example.com/video.mkv", true)]
    [InlineData("https://example.com/audio.mp3", true)]
    [InlineData("https://youtube.com/watch?v=123", false)]
    [InlineData("not-a-url", false)]
    public void IsDirectMediaUrl_WithVariousInputs_ReturnsExpectedResult(string? url, bool expected)
    {
        var result = UrlValidator.IsDirectMediaUrl(url);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://example.com/video", "https://example.com/video")]
    [InlineData("https://example.com/video/", "https://example.com/video/")]
    [InlineData(null, null)]
    [InlineData("", null)]
    public void SanitizeUrl_WithVariousInputs_ReturnsExpectedResult(string? url, string? expected)
    {
        var result = UrlValidator.SanitizeUrl(url);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "www.youtube.com")]
    [InlineData("https://example.com/video", "example.com")]
    [InlineData(null, null)]
    [InlineData("", null)]
    public void ExtractDomain_WithVariousInputs_ReturnsExpectedResult(string? url, string? expected)
    {
        var result = UrlValidator.ExtractDomain(url);
        Assert.Equal(expected, result);
    }
}
