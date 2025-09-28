using FluentAssertions;
using Marventa.Framework.Core.Models.Storage;
using Marventa.Framework.Core.Models.CDN;
using Marventa.Framework.Infrastructure.Services.FileServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace Marventa.Framework.Tests.FileServices;

public class MockStorageServiceTests
{
    private readonly Mock<ILogger<MockStorageService>> _loggerMock;
    private readonly MockStorageService _storageService;

    public MockStorageServiceTests()
    {
        _loggerMock = new Mock<ILogger<MockStorageService>>();
        _storageService = new MockStorageService(_loggerMock.Object);
    }

    [Fact]
    public async Task UploadFileAsync_Should_ReturnSuccessfulResult()
    {
        // Arrange
        var fileContent = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello"
        using var stream = new MemoryStream(fileContent);
        const string fileName = "test.txt";
        const string contentType = "text/plain";

        // Act
        var result = await _storageService.UploadFileAsync(stream, fileName, contentType);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.FileId.Should().NotBeNullOrEmpty();
        result.FilePath.Should().NotBeNullOrEmpty();
        result.PublicUrl.Should().NotBeNullOrEmpty();
        result.FileSizeBytes.Should().Be(fileContent.Length);
        result.MD5Hash.Should().NotBeNullOrEmpty();
        result.ETag.Should().NotBeNullOrEmpty();
        result.UploadedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task DownloadFileAsync_Should_ReturnFileAfterUpload()
    {
        // Arrange
        var fileContent = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
        using var uploadStream = new MemoryStream(fileContent);
        const string fileName = "test.txt";
        const string contentType = "text/plain";

        var uploadResult = await _storageService.UploadFileAsync(uploadStream, fileName, contentType);

        // Act
        var downloadResult = await _storageService.DownloadFileAsync(uploadResult.FileId);

        // Assert
        downloadResult.Should().NotBeNull();
        downloadResult.Success.Should().BeTrue();
        downloadResult.Content.Should().NotBeNull();
        downloadResult.ContentType.Should().Be(contentType);
        downloadResult.ContentLength.Should().Be(fileContent.Length);
        downloadResult.FileInfo.Should().NotBeNull();
        downloadResult.FileInfo.FileName.Should().Be(fileName);

        // Verify content
        var downloadedContent = new byte[downloadResult.ContentLength];
        await downloadResult.Content.ReadExactlyAsync(downloadedContent, 0, (int)downloadResult.ContentLength);
        downloadedContent.Should().BeEquivalentTo(fileContent);

        downloadResult.Dispose();
    }

    [Fact]
    public async Task DownloadFileAsync_Should_ReturnFailureForNonExistentFile()
    {
        // Arrange
        const string nonExistentFileId = "non-existent-file-id";

        // Act
        var result = await _storageService.DownloadFileAsync(nonExistentFileId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("File not found");
    }

    [Fact]
    public async Task DeleteFileAsync_Should_RemoveFile()
    {
        // Arrange
        var fileContent = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
        using var stream = new MemoryStream(fileContent);
        var uploadResult = await _storageService.UploadFileAsync(stream, "test.txt", "text/plain");

        // Act
        var deleteResult = await _storageService.DeleteFileAsync(uploadResult.FileId);

        // Assert
        deleteResult.Should().NotBeNull();
        deleteResult.Success.Should().BeTrue();
        deleteResult.FileId.Should().Be(uploadResult.FileId);
        deleteResult.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        // Verify file no longer exists
        var existsResult = await _storageService.FileExistsAsync(uploadResult.FileId);
        existsResult.Should().BeFalse();
    }

    [Fact]
    public async Task FileExistsAsync_Should_ReturnTrueForExistingFile()
    {
        // Arrange
        var fileContent = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
        using var stream = new MemoryStream(fileContent);
        var uploadResult = await _storageService.UploadFileAsync(stream, "test.txt", "text/plain");

        // Act
        var exists = await _storageService.FileExistsAsync(uploadResult.FileId);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task FileExistsAsync_Should_ReturnFalseForNonExistentFile()
    {
        // Arrange
        const string nonExistentFileId = "non-existent-file-id";

        // Act
        var exists = await _storageService.FileExistsAsync(nonExistentFileId);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task GetFileInfoAsync_Should_ReturnFileInfo()
    {
        // Arrange
        var fileContent = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
        using var stream = new MemoryStream(fileContent);
        const string fileName = "test.txt";
        const string contentType = "text/plain";
        var uploadResult = await _storageService.UploadFileAsync(stream, fileName, contentType);

        // Act
        var fileInfo = await _storageService.GetFileInfoAsync(uploadResult.FileId);

        // Assert
        fileInfo.Should().NotBeNull();
        fileInfo!.FileId.Should().Be(uploadResult.FileId);
        fileInfo.FileName.Should().Be(fileName);
        fileInfo.ContentType.Should().Be(contentType);
        fileInfo.FileSizeBytes.Should().Be(fileContent.Length);
        fileInfo.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task CopyFileAsync_Should_CreateCopyOfFile()
    {
        // Arrange
        var fileContent = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
        using var stream = new MemoryStream(fileContent);
        var uploadResult = await _storageService.UploadFileAsync(stream, "original.txt", "text/plain");
        const string destinationPath = "copy.txt";

        // Act
        var copyResult = await _storageService.CopyFileAsync(uploadResult.FileId, destinationPath);

        // Assert
        copyResult.Should().NotBeNull();
        copyResult.Success.Should().BeTrue();
        copyResult.NewFileId.Should().NotBeNullOrEmpty();
        copyResult.NewFileId.Should().NotBe(uploadResult.FileId);
        copyResult.DestinationPath.Should().Be(destinationPath);

        // Verify both files exist
        var originalExists = await _storageService.FileExistsAsync(uploadResult.FileId);
        var copyExists = await _storageService.FileExistsAsync(copyResult.NewFileId);
        originalExists.Should().BeTrue();
        copyExists.Should().BeTrue();
    }

    [Fact]
    public async Task GenerateSignedUrlAsync_Should_ReturnSignedUrl()
    {
        // Arrange
        var fileContent = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
        using var stream = new MemoryStream(fileContent);
        var uploadResult = await _storageService.UploadFileAsync(stream, "test.txt", "text/plain");
        var expiration = TimeSpan.FromHours(1);

        // Act
        var signedUrlResult = await _storageService.GenerateSignedUrlAsync(uploadResult.FileId, expiration);

        // Assert
        signedUrlResult.Should().NotBeNull();
        signedUrlResult.Success.Should().BeTrue();
        signedUrlResult.SignedUrl.Should().NotBeNullOrEmpty();
        signedUrlResult.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.Add(expiration), TimeSpan.FromMinutes(1));
        signedUrlResult.Permissions.Should().Be(StoragePermissions.Read);
    }

    [Fact]
    public async Task BulkUploadAsync_Should_UploadAllFiles()
    {
        // Arrange
        var files = new Dictionary<string, Stream>
        {
            ["file1.txt"] = new MemoryStream(new byte[] { 0x31, 0x32, 0x33 }),
            ["file2.txt"] = new MemoryStream(new byte[] { 0x34, 0x35, 0x36 })
        };

        // Act
        var result = await _storageService.BulkUploadAsync(files);

        // Assert
        result.Should().NotBeNull();
        result.TotalFiles.Should().Be(2);
        result.SuccessfulUploads.Should().Be(2);
        result.FailedUploads.Should().Be(0);
        result.Results.Should().HaveCount(2);
        result.Results.Should().ContainKey("file1.txt");
        result.Results.Should().ContainKey("file2.txt");
        result.SuccessRate.Should().Be(1.0);
        result.Duration.Should().BeGreaterThan(TimeSpan.Zero);

        // Cleanup
        foreach (var stream in files.Values)
        {
            stream.Dispose();
        }
    }

    [Fact]
    public async Task GetStorageAnalyticsAsync_Should_ReturnAnalytics()
    {
        // Arrange
        var timeRange = new TimeRange
        {
            StartTime = DateTime.UtcNow.AddDays(-30),
            EndTime = DateTime.UtcNow
        };

        // Act
        var analytics = await _storageService.GetStorageAnalyticsAsync(timeRange);

        // Assert
        analytics.Should().NotBeNull();
        analytics.TimeRange.Should().Be(timeRange);
        analytics.TotalStorageBytes.Should().BeGreaterThanOrEqualTo(0);
        analytics.TotalFileCount.Should().BeGreaterThanOrEqualTo(0);
        analytics.BandwidthStats.Should().NotBeNull();
    }

    [Fact]
    public async Task GetStorageHealthAsync_Should_ReturnHealthyStatus()
    {
        // Act
        var health = await _storageService.GetStorageHealthAsync();

        // Assert
        health.Should().NotBeNull();
        health.Status.Should().Be(HealthStatus.Healthy);
        health.ResponseTime.Should().BeGreaterThan(TimeSpan.Zero);
        health.CheckedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void GetProviderInfo_Should_ReturnMockStorageInfo()
    {
        // Act
        var providerInfo = _storageService.GetProviderInfo();

        // Assert
        providerInfo.Should().NotBeNull();
        providerInfo.Name.Should().Be("MockStorage");
        providerInfo.Version.Should().Be("1.0.0");
        providerInfo.SupportedFeatures.Should().NotBeNull();
        providerInfo.SupportedFeatures.SupportsSignedUrls.Should().BeTrue();
        providerInfo.SupportedFeatures.SupportsBulkOperations.Should().BeTrue();
    }

    [Fact]
    public async Task TestConnectivityAsync_Should_ReturnSuccessfulConnection()
    {
        // Act
        var connectivity = await _storageService.TestConnectivityAsync();

        // Assert
        connectivity.Should().NotBeNull();
        connectivity.Success.Should().BeTrue();
        connectivity.ResponseTime.Should().BeGreaterThan(TimeSpan.Zero);
        connectivity.TestedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        connectivity.Endpoint.Should().Be("mock://storage");
    }
}