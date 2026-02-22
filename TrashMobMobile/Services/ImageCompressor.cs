namespace TrashMobMobile.Services;

using System.Diagnostics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;

/// <summary>
/// Provides client-side image compression to reduce upload sizes on slow connections.
/// Resizes images to a maximum dimension while preserving aspect ratio and compresses as JPEG.
/// </summary>
public static class ImageCompressor
{
    private const int MaxDimension = 1600;
    private const float JpegQuality = 0.85f;

    /// <summary>
    /// Compresses an image file in place. Resizes to max 1600px on the longest dimension,
    /// preserving aspect ratio, and re-encodes as JPEG at 85% quality.
    /// If compression fails, the original file is left untouched.
    /// </summary>
    public static async Task<string> CompressAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            return filePath;
        }

        var tempPath = filePath + ".compressed";

        try
        {
            var originalSize = new FileInfo(filePath).Length;

            Microsoft.Maui.Graphics.IImage image;

            using (var inputStream = File.OpenRead(filePath))
            {
                image = PlatformImage.FromStream(inputStream);
            }

            if (image is null)
            {
                return filePath;
            }

            if (image.Width > MaxDimension || image.Height > MaxDimension)
            {
                image = image.Downsize(MaxDimension, MaxDimension);
            }

            await using (var outputStream = File.Create(tempPath))
            {
                await image.AsStream(ImageFormat.Jpeg, JpegQuality)
                    .CopyToAsync(outputStream, cancellationToken);
            }

            var compressedSize = new FileInfo(tempPath).Length;

            File.Delete(filePath);
            File.Move(tempPath, filePath);

            Debug.WriteLine($"ImageCompressor: {originalSize / 1024}KB -> {compressedSize / 1024}KB " +
                $"({(int)image.Width}x{(int)image.Height})");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ImageCompressor: compression failed, using original: {ex.Message}");
            SentrySdk.CaptureException(ex, scope =>
            {
                scope.SetTag("operation", "image_compression");
            });
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                try { File.Delete(tempPath); }
                catch { /* best effort cleanup */ }
            }
        }

        return filePath;
    }
}
