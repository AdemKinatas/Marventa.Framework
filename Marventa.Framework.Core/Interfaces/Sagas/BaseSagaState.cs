namespace Marventa.Framework.Core.Interfaces.Sagas;

public abstract class BaseSagaState : ISaga
{
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public SagaStatus Status { get; set; } = SagaStatus.Started;
    public string CurrentStep { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
    public List<string> CompletedSteps { get; set; } = new();
    public string? TenantId { get; set; }

    public void SetProperty(string key, object value)
    {
        Properties[key] = value;
        UpdatedAt = DateTime.UtcNow;
    }

    public T? GetProperty<T>(string key)
    {
        return Properties.TryGetValue(key, out var value) && value is T typedValue
            ? typedValue
            : default;
    }
}