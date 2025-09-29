using Marventa.Framework.Core.Models.CDN;
using Marventa.Framework.Core.Models.Storage;

namespace Marventa.Framework.Core.Interfaces.Storage;

/// <summary>
/// Provides unified storage operations across multiple providers with enterprise features
/// </summary>
public interface IMarventaStorage
{
    #region File Operations

    /// <summary>
    /// Uploads a file to the configured storage provider
    /// </summary>
    /// <param name="stream">File content stream</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="options">Upload configuration options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upload result with file information</returns>
    Task<StorageUploadResult> UploadFileAsync(Stream stream, string fileName, string contentType, StorageUploadOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from storage
    /// </summary>
    /// <param name="fileId">Unique file identifier</param>
    /// <param name="options">Download configuration options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File stream and metadata</returns>
    Task<StorageDownloadResult> DownloadFileAsync(string fileId, StorageDownloadOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from storage
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="options">Deletion options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion result</returns>
    Task<StorageDeletionResult> DeleteFileAsync(string fileId, StorageDeletionOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a file exists in storage
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if file exists</returns>
    Task<bool> FileExistsAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets file information without downloading the content
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File metadata information</returns>
    Task<StorageFileInfo?> GetFileInfoAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies a file to a new location
    /// </summary>
    /// <param name="sourceFileId">Source file identifier</param>
    /// <param name="destinationPath">Destination path</param>
    /// <param name="options">Copy options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Copy operation result</returns>
    Task<StorageCopyResult> CopyFileAsync(string sourceFileId, string destinationPath, StorageCopyOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a file to a new location
    /// </summary>
    /// <param name="sourceFileId">Source file identifier</param>
    /// <param name="destinationPath">Destination path</param>
    /// <param name="options">Move options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Move operation result</returns>
    Task<StorageMoveResult> MoveFileAsync(string sourceFileId, string destinationPath, StorageMoveOptions? options = null, CancellationToken cancellationToken = default);

    #endregion

    #region Directory Operations

    /// <summary>
    /// Lists files in a directory with pagination and filtering
    /// </summary>
    /// <param name="directoryPath">Directory path (null for root)</param>
    /// <param name="options">Listing options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of files</returns>
    Task<StorageListResult> ListFilesAsync(string? directoryPath = null, StorageListOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new directory
    /// </summary>
    /// <param name="directoryPath">Directory path to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Directory creation result</returns>
    Task<StorageDirectoryResult> CreateDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a directory and optionally its contents
    /// </summary>
    /// <param name="directoryPath">Directory path</param>
    /// <param name="recursive">Whether to delete contents recursively</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Directory deletion result</returns>
    Task<StorageDirectoryResult> DeleteDirectoryAsync(string directoryPath, bool recursive = false, CancellationToken cancellationToken = default);

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Uploads multiple files in a batch operation
    /// </summary>
    /// <param name="files">Dictionary of file names to streams</param>
    /// <param name="options">Bulk upload options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bulk upload result</returns>
    Task<StorageBulkUploadResult> BulkUploadAsync(Dictionary<string, Stream> files, StorageBulkOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple files in a batch operation
    /// </summary>
    /// <param name="fileIds">Array of file identifiers</param>
    /// <param name="options">Bulk deletion options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bulk deletion result</returns>
    Task<StorageBulkDeletionResult> BulkDeleteAsync(string[] fileIds, StorageBulkOptions? options = null, CancellationToken cancellationToken = default);

    #endregion

    #region Access Control

    /// <summary>
    /// Generates a temporary signed URL for file access
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="expiration">URL expiration time</param>
    /// <param name="permissions">Access permissions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Signed URL result</returns>
    Task<StorageSignedUrlResult> GenerateSignedUrlAsync(string fileId, TimeSpan expiration, StoragePermissions permissions = StoragePermissions.Read, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets access permissions for a file
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="permissions">Access permissions to set</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Permission update result</returns>
    Task<StoragePermissionResult> SetPermissionsAsync(string fileId, StorageAccessControl permissions, CancellationToken cancellationToken = default);

    #endregion

    #region Storage Analytics

    /// <summary>
    /// Gets storage usage statistics
    /// </summary>
    /// <param name="timeRange">Time range for statistics</param>
    /// <param name="groupBy">How to group the statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Storage usage analytics</returns>
    Task<StorageAnalyticsResult> GetStorageAnalyticsAsync(TimeRange? timeRange = null, StorageGroupBy groupBy = StorageGroupBy.Day, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets health status of the storage provider
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Storage health information</returns>
    Task<StorageHealthResult> GetStorageHealthAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Provider Management

    /// <summary>
    /// Gets information about the current storage provider
    /// </summary>
    /// <returns>Storage provider information</returns>
    StorageProviderInfo GetProviderInfo();

    /// <summary>
    /// Tests connectivity to the storage provider
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Connectivity test result</returns>
    Task<StorageConnectivityResult> TestConnectivityAsync(CancellationToken cancellationToken = default);

    #endregion
}