namespace Marventa.Framework.Core.Interfaces.Messaging.Outbox;

public interface IInboxService
{
    Task<bool> IsProcessedAsync(string messageId, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(string messageId, CancellationToken cancellationToken = default);
    Task<IInboxMessage> StoreMessageAsync<T>(string messageId, T message, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<IInboxMessage>> GetFailedMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default);
}