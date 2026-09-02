using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using RFL.Downloader.Core.Abstractions;
using Windows.ApplicationModel.DataTransfer;

namespace RFL.Downloader.App.Pages;

public sealed partial class HomePage : Page
{
    private readonly IDownloadEngine _downloadEngine;

    public HomePage(IDownloadEngine downloadEngine)
    {
        this.InitializeComponent();
        _downloadEngine = downloadEngine;
    }

    private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
    {
        var url = UrlInput.Text;
        if (string.IsNullOrWhiteSpace(url))
        {
            ShowError("Please enter a URL");
            return;
        }

        await AnalyzeUrlAsync(url);
    }

    private async void PasteAnalyzeButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var clipboardContent = Clipboard.GetContent();
            if (clipboardContent.Contains(StandardDataFormats.Text))
            {
                var text = clipboardContent.GetTextAsync().GetResults();
                if (!string.IsNullOrEmpty(text))
                {
                    UrlInput.Text = text;
                    await AnalyzeUrlAsync(text);
                }
                else
                {
                    ShowError("Clipboard does not contain text");
                }
            }
            else
            {
                ShowError("Clipboard does not contain text");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Clipboard error: {ex.Message}");
        }
    }

    private async Task AnalyzeUrlAsync(string url)
    {
        LoadingIndicator.IsActive = true;
        LoadingIndicator.Visibility = Visibility.Visible;
        MediaInfoCard.Visibility = Visibility.Collapsed;

        try
        {
            var mediaInfo = await _downloadEngine.AnalyzeAsync(url);
            DisplayMediaInfo(mediaInfo);
        }
        catch (Exception ex)
        {
            ShowError($"Analysis failed: {ex.Message}");
        }
        finally
        {
            LoadingIndicator.IsActive = false;
            LoadingIndicator.Visibility = Visibility.Collapsed;
        }
    }

    private void DisplayMediaInfo(RFL.Downloader.Models.Media.MediaInfo mediaInfo)
    {
        MediaTitle.Text = mediaInfo.Title;
        MediaUploader.Text = mediaInfo.Uploader;
        MediaDuration.Text = mediaInfo.Duration?.ToString() ?? "Unknown duration";
        
        if (!string.IsNullOrEmpty(mediaInfo.ThumbnailUrl))
        {
            try
            {
                MediaThumbnail.Source = new BitmapImage(new Uri(mediaInfo.ThumbnailUrl));
            }
            catch
            {
                MediaThumbnail.Visibility = Visibility.Collapsed;
            }
        }
        else
        {
            MediaThumbnail.Visibility = Visibility.Collapsed;
        }

        MediaInfoCard.Visibility = Visibility.Visible;
    }

    private void ShowError(string message)
    {
        MediaTitle.Text = "Error";
        MediaUploader.Text = message;
        MediaInfoCard.Visibility = Visibility.Visible;
    }
}
