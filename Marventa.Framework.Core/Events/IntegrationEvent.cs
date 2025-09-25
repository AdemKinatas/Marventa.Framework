namespace Marventa.Framework.Core.Events;

public abstract class IntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
    public string? CorrelationId { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    protected IntegrationEvent()
    {
    }

    protected IntegrationEvent(string correlationId)
    {
        CorrelationId = correlationId;
    }
}