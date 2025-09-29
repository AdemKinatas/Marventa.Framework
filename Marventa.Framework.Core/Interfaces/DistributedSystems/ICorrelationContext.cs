namespace Marventa.Framework.Core.Interfaces.DistributedSystems;

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