namespace Marventa.Framework.Core.Interfaces;

public interface ICorrelationContext
{
    string CorrelationId { get; }
    string? UserId { get; }
    string? TenantId { get; }
    Dictionary<string, object> Properties { get; }
    void SetCorrelationId(string correlationId);
    void SetUserId(string userId);
    void SetTenantId(string tenantId);
    void SetProperty(string key, object value);
    T? GetProperty<T>(string key);
}

public interface IActivityService
{
    IDisposable? StartActivity(string name);
    IDisposable? StartActivity(string name, Dictionary<string, object?> tags);
    void AddTag(string key, object? value);
    void AddEvent(string name);
    void AddEvent(string name, Dictionary<string, object?> tags);
    void SetStatus(bool success, string? description = null);
}