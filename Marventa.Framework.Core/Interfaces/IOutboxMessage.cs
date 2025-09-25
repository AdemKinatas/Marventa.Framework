namespace Marventa.Framework.Core.Interfaces;

public interface IOutboxMessage
{
    Guid Id { get; }
    string Type { get; }
    string Data { get; }
    DateTime CreatedAt { get; }
    DateTime? ProcessedAt { get; }
    string? Error { get; }
    int RetryCount { get; }
    string? IdempotencyKey { get; }
    string? TenantId { get; }
    Dictionary<string, object> Headers { get; }
}

public interface IInboxMessage
{
    Guid Id { get; }
    string MessageId { get; }
    string Type { get; }
    string Data { get; }
    DateTime ReceivedAt { get; }
    DateTime? ProcessedAt { get; }
    bool IsProcessed { get; }
    string? Error { get; }
    int RetryCount { get; }
    string? TenantId { get; }
    Dictionary<string, object> Headers { get; }
}

public interface IOutboxService
{
    Task PublishAsync<T>(T message, string? idempotencyKey = null, CancellationToken cancellationToken = default) where T : class;
    Task PublishManyAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<IOutboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default);
}

public interface IInboxService
{
    Task<bool> IsProcessedAsync(string messageId, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(string messageId, CancellationToken cancellationToken = default);
    Task<IInboxMessage> StoreMessageAsync<T>(string messageId, T message, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<IInboxMessage>> GetFailedMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default);
}

public interface ITransactionalMessageService
{
    Task PublishAsync<T>(T message, string? idempotencyKey = null, CancellationToken cancellationToken = default) where T : class;
    Task<bool> HandleInboxMessageAsync<T>(string messageId, T message, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class;
}