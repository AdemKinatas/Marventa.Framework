using System.ComponentModel.DataAnnotations;
using Marventa.Framework.Core.Entities;
using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Domain.Entities;

public class SagaStepExecution : BaseEntity
{
    [Required]
    public Guid SagaCorrelationId { get; set; }

    [Required]
    [MaxLength(100)]
    public string StepName { get; set; } = string.Empty;

    [Required]
    public SagaStepStatus Status { get; set; } = SagaStepStatus.Started;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    public int RetryCount { get; set; } = 0;
    public int MaxRetries { get; set; } = 3;

    public string? TenantId { get; set; }

    // Navigation property
    public SagaState? SagaState { get; set; }
}

public enum SagaStepStatus
{
    Started,
    Completed,
    Failed,
    Compensating,
    Compensated,
    Skipped
}