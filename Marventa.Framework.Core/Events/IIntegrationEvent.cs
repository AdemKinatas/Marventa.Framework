namespace Marventa.Framework.Core.Events;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
    string? CorrelationId { get; }
    Dictionary<string, object> Metadata { get; }
}