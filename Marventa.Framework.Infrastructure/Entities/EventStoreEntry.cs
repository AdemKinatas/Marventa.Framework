using System.ComponentModel.DataAnnotations;
using Marventa.Framework.Core.Entities;

namespace Marventa.Framework.Infrastructure.Entities;

public class EventStoreEntry : BaseEntity
{
    [Required]
    public Guid StreamId { get; set; }

    [Required]
    [MaxLength(200)]
    public string EventType { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string AssemblyName { get; set; } = string.Empty;

    [Required]
    public string EventData { get; set; } = string.Empty;

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public long SequenceNumber { get; set; }

    [MaxLength(100)]
    public string? CorrelationId { get; set; }

    [MaxLength(100)]
    public string? CausationId { get; set; }

    public string? TenantId { get; set; }

    [MaxLength(100)]
    public string? UserId { get; set; }

    public Dictionary<string, object> Metadata { get; set; } = new();
}