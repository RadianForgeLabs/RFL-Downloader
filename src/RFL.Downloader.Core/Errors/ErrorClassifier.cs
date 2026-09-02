/*
 * RFL Downloader
 * Copyright (c) 2026 RADIANFORGELABS / RFL Studios
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

using RFL.Downloader.Models.Errors;

namespace RFL.Downloader.Core.Errors;

public static class ErrorClassifier
{
    public static DownloadError ClassifyError(Exception exception)
    {
        return exception switch
        {
            UriFormatException => new DownloadError
            {
                ErrorType = ErrorType.InvalidUrl,
                Message = "The provided URL is not valid.",
                TechnicalDetails = exception.Message,
                CanRetry = false
            },
            InvalidOperationException => new DownloadError
            {
                ErrorType = ErrorType.EngineError,
                Message = "An error occurred with the download engine.",
                TechnicalDetails = exception.Message,
                CanRetry = true
            },
            OperationCanceledException => new DownloadError
            {
                ErrorType = ErrorType.Cancelled,
                Message = "The operation was cancelled.",
                TechnicalDetails = exception.Message,
                CanRetry = true
            },
            UnauthorizedAccessException => new DownloadError
            {
                ErrorType = ErrorType.PermissionDenied,
                Message = "Permission denied. Check file and folder permissions.",
                TechnicalDetails = exception.Message,
                CanRetry = false
            },
            IOException ioException when ioException.Message.Contains("disk") || ioException.Message.Contains("space") => new DownloadError
            {
                ErrorType = ErrorType.DiskFull,
                Message = "Disk full. Free up space and try again.",
                TechnicalDetails = exception.Message,
                CanRetry = true
            },
            TimeoutException => new DownloadError
            {
                ErrorType = ErrorType.NetworkError,
                Message = "Network timeout. Check your connection.",
                TechnicalDetails = exception.Message,
                CanRetry = true
            },
            _ => new DownloadError
            {
                ErrorType = ErrorType.UnknownError,
                Message = "An unexpected error occurred.",
                TechnicalDetails = exception.Message,
                CanRetry = true
            }
        };
    }

    public static DownloadError ClassifyFromEngineOutput(string output)
    {
        var lowerOutput = output.ToLowerInvariant();

        if (lowerOutput.Contains("unsupported url") || lowerOutput.Contains("unsupported site"))
        {
            return new DownloadError
            {
                ErrorType = ErrorType.UnsupportedSite,
                Message = "This URL is not supported by the download engine.",
                TechnicalDetails = output,
                CanRetry = false
            };
        }

        if (lowerOutput.Contains("no video formats found") || lowerOutput.Contains("no formats"))
        {
            return new DownloadError
            {
                ErrorType = ErrorType.NoFormatsAvailable,
                Message = "No video formats are available for this content.",
                TechnicalDetails = output,
                CanRetry = false
            };
        }

        if (lowerOutput.Contains("http error") || lowerOutput.Contains("network") || lowerOutput.Contains("connection"))
        {
            return new DownloadError
            {
                ErrorType = ErrorType.NetworkError,
                Message = "A network error occurred. Check your connection.",
                TechnicalDetails = output,
                CanRetry = true
            };
        }

        if (lowerOutput.Contains("sign in") || lowerOutput.Contains("login") || lowerOutput.Contains("authentication"))
        {
            return new DownloadError
            {
                ErrorType = ErrorType.AuthenticationRequired,
                Message = "Authentication is required to download this content.",
                TechnicalDetails = output,
                CanRetry = false
            };
        }

        if (lowerOutput.Contains("rate limit") || lowerOutput.Contains("too many requests"))
        {
            return new DownloadError
            {
                ErrorType = ErrorType.RateLimited,
                Message = "Rate limited. Please wait and try again later.",
                TechnicalDetails = output,
                CanRetry = true
            };
        }

        return new DownloadError
        {
            ErrorType = ErrorType.EngineError,
            Message = "The download engine encountered an error.",
            TechnicalDetails = output,
            CanRetry = true
        };
    }
}
