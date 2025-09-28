using FluentAssertions;
using Marventa.Framework.Core.Models.FileProcessing;
using Marventa.Framework.Infrastructure.Services.FileServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace Marventa.Framework.Tests.FileServices;

public class MockFileProcessorTests
{
    private readonly Mock<ILogger<MockFileProcessor>> _loggerMock;
    private readonly MockFileProcessor _fileProcessor;

    public MockFileProcessorTests()
    {
        _loggerMock = new Mock<ILogger<MockFileProcessor>>();
        _fileProcessor = new MockFileProcessor(_loggerMock.Object);
    }

    [Fact]
    public async Task ProcessImageAsync_Should_ReturnSuccessfulResult()
    {
        // Arrange
        using var inputStream = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF }); // Mock JPEG header
        var options = new ProcessingOptions
        {
            TargetWidth = 800,
            TargetHeight = 600,
            Quality = 85
        };

        // Act
        var result = await _fileProcessor.ProcessImageAsync(inputStream, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.ProcessedImage.Should().NotBeNull();
        result.OriginalSize.Should().BeGreaterThan(0);
        result.ProcessedSize.Should().BeGreaterThan(0);
        result.Dimensions.Width.Should().Be(800);
        result.Dimensions.Height.Should().Be(600);
        result.CompressionRatio.Should().BeGreaterThan(0);
        result.ProcessingTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task GenerateThumbnailsAsync_Should_ReturnThumbnailsForAllSizes()
    {
        // Arrange
        using var inputStream = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF });
        var sizes = new[]
        {
            new ThumbnailSize { Name = "small", Width = 150, Height = 150 },
            new ThumbnailSize { Name = "medium", Width = 300, Height = 300 }
        };

        // Act
        var result = await _fileProcessor.GenerateThumbnailsAsync(inputStream, sizes);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Thumbnails.Should().HaveCount(2);
        result.Thumbnails.Should().ContainKey("small");
        result.Thumbnails.Should().ContainKey("medium");
        result.Thumbnails["small"].Dimensions.Width.Should().Be(150);
        result.Thumbnails["medium"].Dimensions.Width.Should().Be(300);
        result.ProcessingTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task OptimizeImageAsync_Should_ReturnOptimizedResult()
    {
        // Arrange
        using var inputStream = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF });
        var options = new OptimizationOptions
        {
            Quality = 75,
            EnableProgressive = true
        };

        // Act
        var result = await _fileProcessor.OptimizeImageAsync(inputStream, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.OptimizedImage.Should().NotBeNull();
        result.OriginalSizeBytes.Should().BeGreaterThan(0);
        result.OptimizedSizeBytes.Should().BeGreaterThan(0);
        result.CompressionRatio.Should().BeInRange(0, 1);
        result.QualityScore.Should().BeInRange(0, 1);
        result.ProcessingTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task ApplyWatermarkAsync_Should_ReturnWatermarkedResult()
    {
        // Arrange
        using var inputStream = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF });
        var options = new WatermarkOptions
        {
            Text = "Test Watermark",
            Position = WatermarkPosition.BottomRight,
            Opacity = 0.5f
        };

        // Act
        var result = await _fileProcessor.ApplyWatermarkAsync(inputStream, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.WatermarkedImage.Should().NotBeNull();
        result.WatermarkApplied.Should().BeTrue();
        result.ProcessingTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task ConvertFormatAsync_Should_ReturnConvertedResult()
    {
        // Arrange
        using var inputStream = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF });
        const string targetFormat = "png";

        // Act
        var result = await _fileProcessor.ConvertFormatAsync(inputStream, targetFormat, null, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.ConvertedImage.Should().NotBeNull();
        result.SourceFormat.Should().NotBeNullOrEmpty();
        result.TargetFormat.Should().Be(targetFormat);
        result.ProcessingTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task ValidateImageAsync_Should_ReturnValidationResult()
    {
        // Arrange
        using var inputStream = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF });
        var options = new ValidationOptions
        {
            CheckFormat = true,
            CheckDimensions = true
        };

        // Act
        var result = await _fileProcessor.ValidateImageAsync(inputStream, options);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Format.Should().NotBeNullOrEmpty();
        result.Dimensions.Should().NotBeNull();
        result.FileSizeBytes.Should().BeGreaterThan(0);
        result.ColorDepth.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ProcessBatchAsync_Should_ProcessAllImages()
    {
        // Arrange
        var inputs = new Dictionary<string, Stream>
        {
            ["image1"] = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF }),
            ["image2"] = new MemoryStream(new byte[] { 0x89, 0x50, 0x4E, 0x47 })
        };
        var options = new ProcessingOptions
        {
            TargetWidth = 800,
            TargetHeight = 600
        };

        // Act
        var result = await _fileProcessor.ProcessBatchAsync(inputs, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.TotalProcessed.Should().Be(2);
        result.SuccessfullyProcessed.Should().Be(2);
        result.Failed.Should().Be(0);
        result.Results.Should().HaveCount(2);
        result.Results.Should().ContainKey("image1");
        result.Results.Should().ContainKey("image2");
        result.TotalProcessingTime.Should().BeGreaterThan(TimeSpan.Zero);

        // Cleanup
        foreach (var stream in inputs.Values)
        {
            stream.Dispose();
        }
    }

    [Theory]
    [InlineData(100, 100)]
    [InlineData(800, 600)]
    [InlineData(1920, 1080)]
    public async Task ProcessImageAsync_Should_HandleDifferentTargetSizes(int width, int height)
    {
        // Arrange
        using var inputStream = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF });
        var options = new ProcessingOptions
        {
            TargetWidth = width,
            TargetHeight = height
        };

        // Act
        var result = await _fileProcessor.ProcessImageAsync(inputStream, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Dimensions.Width.Should().Be(width);
        result.Dimensions.Height.Should().Be(height);
    }
}