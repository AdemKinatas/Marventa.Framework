namespace Marventa.Framework.Core.Interfaces.Messaging.Outbox;

public interface IOutboxService
{
    Task PublishAsync<T>(T message, string? idempotencyKey = null, CancellationToken cancellationToken = default) where T : class;
    Task PublishManyAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<IOutboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default);
}