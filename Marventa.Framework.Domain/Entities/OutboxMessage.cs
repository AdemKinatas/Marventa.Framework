using System.Text.Json;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Core.Entities;

namespace Marventa.Framework.Domain.Entities;

public class OutboxMessage : BaseEntity, IOutboxMessage
{
    public string Type { get; set; } = string.Empty;

    // Explicit interface implementation for Guid to string conversion
    Guid IOutboxMessage.Id => base.Id;
    public string Data { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? IdempotencyKey { get; set; }
    public string? TenantId { get; set; }
    public string MessageType => Type;  // Alias for compatibility
    public string MessageData => Data;  // Alias for compatibility
    public string HeadersJson { get; set; } = "{}";

    public Dictionary<string, object> Headers
    {
        get => string.IsNullOrEmpty(HeadersJson)
            ? new Dictionary<string, object>()
            : JsonSerializer.Deserialize<Dictionary<string, object>>(HeadersJson) ?? new Dictionary<string, object>();
        set => HeadersJson = JsonSerializer.Serialize(value);
    }

    public bool IsProcessed => ProcessedAt.HasValue;
    public bool HasFailed => !string.IsNullOrEmpty(Error);
    public bool ShouldRetry => RetryCount < 3 && !IsProcessed;

    public static OutboxMessage Create<T>(T message, string? idempotencyKey = null, string? tenantId = null, Dictionary<string, object>? headers = null)
        where T : class
    {
        return new OutboxMessage
        {
            Type = typeof(T).Name,
            Data = JsonSerializer.Serialize(message),
            IdempotencyKey = idempotencyKey,
            TenantId = tenantId,
            Headers = headers ?? new Dictionary<string, object>(),
            CreatedAt = DateTime.UtcNow
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