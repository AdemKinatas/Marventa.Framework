namespace Marventa.Framework.Core.Interfaces.Messaging.Outbox;

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