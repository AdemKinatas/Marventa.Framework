namespace Marventa.Framework.Features.Idempotency;

/// <summary>
/// Service for managing idempotent operations using cached request/response pairs.
/// Ensures that duplicate requests with the same idempotency key return the same result.
/// </summary>
public interface IIdempotencyService
{
    /// <summary>
    /// Checks if a request with the given idempotency key has already been processed.
    /// </summary>
    /// <param name="key">The idempotency key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the request has been processed, otherwise false.</returns>
    Task<bool> IsProcessedAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the cached response for a given idempotency key.
    /// </summary>
    /// <param name="key">The idempotency key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached response if found, otherwise null.</returns>
    Task<IdempotentResponse?> GetResponseAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a response for the given idempotency key.
    /// </summary>
    /// <param name="key">The idempotency key.</param>
    /// <param name="response">The response to cache.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetResponseAsync(string key, IdempotentResponse response, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a cached idempotent response.
/// </summary>
public class IdempotentResponse
{
    /// <summary>
    /// Gets or sets the HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the response body.
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Gets or sets the response headers.
    /// </summary>
    public Dictionary<string, string[]> Headers { get; set; } = new();

    /// <summary>
    /// Gets or sets the content type.
    /// </summary>
    public string? ContentType { get; set; }
}
