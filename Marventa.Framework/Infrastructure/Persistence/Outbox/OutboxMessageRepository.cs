using Microsoft.EntityFrameworkCore;

namespace Marventa.Framework.Infrastructure.Persistence.Outbox;

/// <summary>
/// Entity Framework Core implementation of the outbox message repository.
/// </summary>
public class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly DbContext _context;
    private readonly DbSet<OutboxMessage> _outboxMessages;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxMessageRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public OutboxMessageRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _outboxMessages = context.Set<OutboxMessage>();
    }

    /// <inheritdoc/>
    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        await _outboxMessages.AddAsync(message, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        if (batchSize <= 0)
            throw new ArgumentException("Batch size must be greater than 0.", nameof(batchSize));

        return await _outboxMessages
            .Where(m => m.ProcessedOn == null && m.RetryCount < m.MaxRetries)
            .OrderBy(m => m.OccurredOn)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _outboxMessages.FindAsync(new object[] { messageId }, cancellationToken);

        if (message == null)
            throw new InvalidOperationException($"Outbox message with ID {messageId} not found.");

        message.ProcessedOn = DateTime.UtcNow;
        message.Error = null;
    }

    /// <inheritdoc/>
    public async Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Error message cannot be null or empty.", nameof(error));

        var message = await _outboxMessages.FindAsync(new object[] { messageId }, cancellationToken);

        if (message == null)
            throw new InvalidOperationException($"Outbox message with ID {messageId} not found.");

        message.RetryCount++;
        message.Error = error;
    }

    /// <inheritdoc/>
    public async Task DeleteProcessedMessagesAsync(TimeSpan retentionPeriod, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.Subtract(retentionPeriod);

        var processedMessages = await _outboxMessages
            .Where(m => m.ProcessedOn != null && m.ProcessedOn < cutoffDate)
            .ToListAsync(cancellationToken);

        _outboxMessages.RemoveRange(processedMessages);
    }
}
