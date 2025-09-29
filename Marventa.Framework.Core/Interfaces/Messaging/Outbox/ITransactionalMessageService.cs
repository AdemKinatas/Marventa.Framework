namespace Marventa.Framework.Core.Interfaces.Messaging.Outbox;

public interface ITransactionalMessageService
{
    Task PublishAsync<T>(T message, string? idempotencyKey = null, CancellationToken cancellationToken = default) where T : class;
    Task<bool> HandleInboxMessageAsync<T>(string messageId, T message, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class;
}