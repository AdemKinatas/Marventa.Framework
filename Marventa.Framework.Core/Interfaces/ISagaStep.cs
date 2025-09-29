namespace Marventa.Framework.Core.Interfaces;

public interface ISagaStep<TSaga> where TSaga : class, ISaga
{
    string StepName { get; }
    Task<SagaStepResult> ExecuteAsync(TSaga saga, object @event, CancellationToken cancellationToken = default);
    Task<SagaStepResult> CompensateAsync(TSaga saga, string reason, CancellationToken cancellationToken = default);
}

public class SagaStepResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public object? NextEvent { get; set; }
    public TimeSpan? Delay { get; set; }
    public bool RequiresCompensation { get; set; }

    public static SagaStepResult Success(object? nextEvent = null, TimeSpan? delay = null)
        => new() { IsSuccess = true, NextEvent = nextEvent, Delay = delay };

    public static SagaStepResult Failed(string errorMessage, bool requiresCompensation = true)
        => new() { IsSuccess = false, ErrorMessage = errorMessage, RequiresCompensation = requiresCompensation };

    public static SagaStepResult Compensated()
        => new() { IsSuccess = true, RequiresCompensation = false };
}

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