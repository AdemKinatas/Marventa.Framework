using System.Text.Json;
using Marventa.Framework.Core.Interfaces.Messaging.Outbox;
using Marventa.Framework.Core.Entities;

namespace Marventa.Framework.Domain.Entities;

public class InboxMessage : BaseEntity, IInboxMessage
{
    public string MessageId { get; set; } = string.Empty;

    // Explicit interface implementation for Guid to string conversion
    Guid IInboxMessage.Id => base.Id;
    public string Type { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public bool IsProcessed { get; set; } = false;
    public string? Error { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? TenantId { get; set; }
    public string HeadersJson { get; set; } = "{}";

    public Dictionary<string, object> Headers
    {
        get => string.IsNullOrEmpty(HeadersJson)
            ? new Dictionary<string, object>()
            : JsonSerializer.Deserialize<Dictionary<string, object>>(HeadersJson) ?? new Dictionary<string, object>();
        set => HeadersJson = JsonSerializer.Serialize(value);
    }

    public bool HasFailed => !string.IsNullOrEmpty(Error);
    public bool ShouldRetry => RetryCount < 3 && !IsProcessed;

    public static InboxMessage Create<T>(string messageId, T message, string? tenantId = null, Dictionary<string, object>? headers = null)
        where T : class
    {
        return new InboxMessage
        {
            MessageId = messageId,
            Type = typeof(T).Name,
            Data = JsonSerializer.Serialize(message),
            TenantId = tenantId,
            Headers = headers ?? new Dictionary<string, object>(),
            ReceivedAt = DateTime.UtcNow
        };
    }

    public T? GetMessage<T>() where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(Data);
        }
        catch
        {
            return null;
        }
    }
}