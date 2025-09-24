using System;

namespace Marventa.Framework.Domain.Events;

public abstract class DomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType { get; }

    protected DomainEvent()
    {
        EventType = GetType().Name;
    }
}