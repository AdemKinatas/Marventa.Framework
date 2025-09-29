using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Core.Entities;

namespace Marventa.Framework.Domain.Entities;

public class SagaState : BaseEntity, ISaga
{
    [Required]
    public Guid CorrelationId { get; set; }

    [Required]
    [MaxLength(100)]
    public string SagaType { get; set; } = string.Empty;

    [Required]
    public SagaStatus Status { get; set; } = SagaStatus.Started;

    [MaxLength(100)]
    public string CurrentStep { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    [Required]
    public string Data { get; set; } = "{}";

    public string? TenantId { get; set; }

    public List<string> CompletedSteps { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    // Computed property for deserialization
    public T? GetData<T>() where T : class
    {
        try
        {
            return string.IsNullOrEmpty(Data) ? null : JsonSerializer.Deserialize<T>(Data);
        }
        catch
        {
            return null;
        }
    }

    public void SetData<T>(T data) where T : class
    {
        Data = JsonSerializer.Serialize(data);
        UpdatedDate = DateTime.UtcNow;
    }
}