namespace Marventa.Framework.Core.Interfaces.Messaging.Outbox;

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