namespace Marventa.Framework.Infrastructure.Persistence.Outbox;

/// <summary>
/// Repository interface for managing outbox messages.
/// </summary>
public interface IOutboxMessageRepository
{
    /// <summary>
    /// Adds an outbox message to the repository.
    /// </summary>
    /// <param name="message">The outbox message to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a batch of unprocessed messages for processing.
    /// </summary>
    /// <param name="batchSize">The maximum number of messages to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of unprocessed outbox messages.</returns>
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as processed.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as failed and increments retry count.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <param name="error">The error message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes processed messages older than the specified retention period.
    /// </summary>
    /// <param name="retentionPeriod">The retention period for processed messages.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteProcessedMessagesAsync(TimeSpan retentionPeriod, CancellationToken cancellationToken = default);
}
