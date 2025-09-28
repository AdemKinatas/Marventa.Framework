using Marventa.Framework.Core.Models.FileProcessing;

namespace Marventa.Framework.Core.Interfaces;

/// <summary>
/// Provides advanced file processing capabilities including image manipulation, optimization, and format conversion
/// </summary>
public interface IMarventaFileProcessor
{
    /// <summary>
    /// Processes an image with specified options (resize, rotate, crop, etc.)
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="options">Processing configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result with output stream and metadata</returns>
    Task<ProcessingResult> ProcessImageAsync(Stream image, ProcessingOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates multiple thumbnail sizes from an image
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="sizes">Array of thumbnail sizes to generate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing all generated thumbnails</returns>
    Task<ThumbnailResult> GenerateThumbnailsAsync(Stream image, ThumbnailSize[] sizes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Optimizes an image for web delivery with compression and quality settings
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="level">Optimization level (low, medium, high, maximum)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Optimization result with compressed image and savings info</returns>
    Task<OptimizationResult> OptimizeImageAsync(Stream image, OptimizationLevel level, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies watermark to an image with positioning and opacity options
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="options">Watermark configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with watermarked image</returns>
    Task<WatermarkResult> ApplyWatermarkAsync(Stream image, WatermarkOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Converts image format (JPEG, PNG, WebP, AVIF, etc.)
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="targetFormat">Target format (jpg, png, webp, avif)</param>
    /// <param name="quality">Output quality (1-100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Conversion result with new format</returns>
    Task<ConversionResult> ConvertFormatAsync(Stream image, string targetFormat, int quality = 85, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if the file is a valid image and extracts basic metadata
    /// </summary>
    /// <param name="file">Input file stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with image properties</returns>
    Task<ImageValidationResult> ValidateImageAsync(Stream file, CancellationToken cancellationToken = default);
}