using Marventa.Framework.Core.Interfaces.Events;

namespace Marventa.Framework.Core.Events;

public abstract class DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
}