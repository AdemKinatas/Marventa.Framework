namespace Marventa.Framework.Infrastructure.Persistence.Outbox;

/// <summary>
/// Represents an outbox message for reliable event publishing using the Transactional Outbox pattern.
/// Stores domain events in the database within the same transaction as business data.
/// </summary>
public class OutboxMessage
{
    /// <summary>
    /// Gets or sets the unique identifier for the outbox message.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the fully qualified type name of the event.
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serialized JSON payload of the event.
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the message was successfully processed.
    /// Null indicates the message has not been processed yet.
    /// </summary>
    public DateTime? ProcessedOn { get; set; }

    /// <summary>
    /// Gets or sets the error message if processing failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the number of times processing has been retried.
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of retry attempts allowed.
    /// Default is 3.
    /// </summary>
    public int MaxRetries { get; set; } = 3;
}
