namespace Marventa.Framework.Core.Events;

public abstract record BaseEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string EventType { get; init; } = string.Empty;
    public Dictionary<string, object> Metadata { get; init; } = new();

    protected BaseEvent()
    {
        EventType = GetType().Name;
    }
}

public abstract record BaseCommand
{
    public Guid CommandId { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public string CommandType { get; init; } = string.Empty;
    public string? CorrelationId { get; init; }
    public Dictionary<string, object> Metadata { get; init; } = new();

    protected BaseCommand()
    {
        CommandType = GetType().Name;
    }
}