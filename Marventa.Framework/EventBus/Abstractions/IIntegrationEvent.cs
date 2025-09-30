namespace Marventa.Framework.EventBus.Abstractions;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}
