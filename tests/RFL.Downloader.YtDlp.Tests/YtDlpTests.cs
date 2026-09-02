using RFL.Downloader.Core.Validation;
using Xunit;

namespace RFL.Downloader.YtDlp.Tests;

public class YtDlpTests
{
    [Fact]
    public void UrlValidator_ShouldValidateYouTubeUrls()
    {
        var validUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
        Assert.True(UrlValidator.IsValidUrl(validUrl));
        Assert.True(UrlValidator.IsYoutubeUrl(validUrl));
    }

    [Fact]
    public void UrlValidator_ShouldRejectInvalidUrls()
    {
        var invalidUrl = "not-a-valid-url";
        Assert.False(UrlValidator.IsValidUrl(invalidUrl));
        Assert.False(UrlValidator.IsYoutubeUrl(invalidUrl));
    }
}
