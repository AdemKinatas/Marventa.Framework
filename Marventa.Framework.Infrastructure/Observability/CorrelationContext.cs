using System.Diagnostics;
using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Infrastructure.Observability;

public class CorrelationContext : ICorrelationContext
{
    private readonly AsyncLocal<CorrelationState> _state = new();

    public string CorrelationId => _state.Value?.CorrelationId ?? GenerateCorrelationId();
    public string? UserId => _state.Value?.UserId;
    public string? TenantId => _state.Value?.TenantId;
    public Dictionary<string, object> Properties => _state.Value?.Properties ?? new Dictionary<string, object>();

    public void SetCorrelationId(string correlationId)
    {
        EnsureState();
        _state.Value!.CorrelationId = correlationId;
        Activity.Current?.SetTag("correlation.id", correlationId);
    }

    public void SetUserId(string userId)
    {
        EnsureState();
        _state.Value!.UserId = userId;
        Activity.Current?.SetTag("user.id", userId);
    }

    public void SetTenantId(string tenantId)
    {
        EnsureState();
        _state.Value!.TenantId = tenantId;
        Activity.Current?.SetTag("tenant.id", tenantId);
    }

    public void SetProperty(string key, object value)
    {
        EnsureState();
        _state.Value!.Properties[key] = value;
        Activity.Current?.SetTag($"correlation.{key}", value?.ToString());
    }

    public T? GetProperty<T>(string key)
    {
        var properties = _state.Value?.Properties;
        if (properties?.TryGetValue(key, out var value) == true && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    private void EnsureState()
    {
        _state.Value ??= new CorrelationState { CorrelationId = GenerateCorrelationId() };
    }

    private static string GenerateCorrelationId()
    {
        return Activity.Current?.Id ?? Guid.NewGuid().ToString("N")[..16];
    }

    private class CorrelationState
    {
        public string CorrelationId { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}