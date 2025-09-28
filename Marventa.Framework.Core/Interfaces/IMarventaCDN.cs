using Marventa.Framework.Core.Models.CDN;

namespace Marventa.Framework.Core.Interfaces;

/// <summary>
/// Provides Content Delivery Network (CDN) management capabilities for file distribution and caching
/// </summary>
public interface IMarventaCDN
{
    /// <summary>
    /// Uploads a file to the CDN for global distribution
    /// </summary>
    /// <param name="fileId">Unique identifier for the file</param>
    /// <param name="content">File content stream</param>
    /// <param name="contentType">MIME type of the content</param>
    /// <param name="options">CDN upload options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>CDN upload result with URLs and metadata</returns>
    Task<CDNUploadResult> UploadToCDNAsync(string fileId, Stream content, string contentType, CDNUploadOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cached content at specified URLs to force refresh
    /// </summary>
    /// <param name="urls">Array of URLs to invalidate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Invalidation result with status for each URL</returns>
    Task<CDNInvalidationResult> InvalidateCacheAsync(string[] urls, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pre-loads content into edge caches for improved performance
    /// </summary>
    /// <param name="urls">Array of URLs to warm up</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cache warming result with status for each URL</returns>
    Task<CDNWarmupResult> WarmCacheAsync(string[] urls, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves performance and usage metrics for a file
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="timeRange">Time range for metrics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed metrics including bandwidth, requests, and geographic distribution</returns>
    Task<CDNMetrics> GetMetricsAsync(string fileId, TimeRange timeRange, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the optimized CDN URL for a file with optional transformations
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="transformations">Optional URL transformations</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Optimized CDN URL</returns>
    Task<string> GetCDNUrlAsync(string fileId, URLTransformations? transformations = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Configures caching rules for specific content types or patterns
    /// </summary>
    /// <param name="rules">Array of caching rules to apply</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Configuration result</returns>
    Task<CDNConfigurationResult> ConfigureCachingRulesAsync(CachingRule[] rules, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a file from the CDN and all edge caches
    /// </summary>
    /// <param name="fileId">File identifier to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion result</returns>
    Task<CDNDeletionResult> DeleteFromCDNAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed information about CDN distribution status
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Distribution status across edge locations</returns>
    Task<CDNDistributionStatus> GetDistributionStatusAsync(string fileId, CancellationToken cancellationToken = default);
}