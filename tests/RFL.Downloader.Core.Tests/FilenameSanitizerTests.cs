using RFL.Downloader.Core.Security;
using Xunit;

namespace RFL.Downloader.Core.Tests;

public class FilenameSanitizerTests
{
    [Theory]
    [InlineData("normal_file.txt", "normal_file.txt")]
    [InlineData("file:with:colons.txt", "file_with_colons.txt")]
    [InlineData("file/with/slashes.txt", "file_with_slashes.txt")]
    [InlineData("file\\with\\backslashes.txt", "file_with_backslashes.txt")]
    [InlineData("file*with?wildcards.txt", "file_with_wildcards.txt")]
    [InlineData("file|with|pipes.txt", "file_with_pipes.txt")]
    [InlineData("file\"with\"quotes.txt", "file_with_quotes.txt")]
    [InlineData("file<with>brackets.txt", "file_with_brackets.txt")]
    [InlineData("", "download")]
    [InlineData("   ", "download")]
    [InlineData(null, "download")]
    public void SanitizeFilename_WithVariousInputs_ReturnsSanitizedResult(string? filename, string expected)
    {
        var result = FilenameSanitizer.SanitizeFilename(filename);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("CON", "_CON")]
    [InlineData("PRN", "_PRN")]
    [InlineData("AUX", "_AUX")]
    [InlineData("NUL", "_NUL")]
    [InlineData("COM1", "_COM1")]
    [InlineData("LPT1", "_LPT1")]
    [InlineData("normal.txt", "normal.txt")]
    public void SanitizeFilename_WithReservedNames_PrefixesWithUnderscore(string filename, string expected)
    {
        var result = FilenameSanitizer.SanitizeFilename(filename);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("C:\\Users\\Test\\file.txt", true)]
    [InlineData("/home/user/file.txt", true)]
    [InlineData("relative\\path\\file.txt", true)]
    [InlineData("invalid<>path", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidPath_WithVariousInputs_ReturnsExpectedResult(string? path, bool expected)
    {
        var result = FilenameSanitizer.IsValidPath(path);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("normal\\path", false)]
    [InlineData("path\\..\\file.txt", true)]
    [InlineData("path/../../file.txt", true)]
    [InlineData("normal", false)]
    public void IsPathTraversal_WithVariousInputs_ReturnsExpectedResult(string path, bool expected)
    {
        var result = FilenameSanitizer.IsPathTraversal(path);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void SafeCombinePath_WithTraversal_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            FilenameSanitizer.SafeCombinePath("C:\\Base", "..\\traversal"));
    }

    [Fact]
    public void SafeCombinePath_WithValidPaths_ReturnsCombinedPath()
    {
        var result = FilenameSanitizer.SafeCombinePath("C:\\Base", "subfolder\\file.txt");
        Assert.Contains("subfolder", result);
        Assert.Contains("file.txt", result);
    }
}
