namespace Marventa.Framework.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;

    void Subscribe<TEvent, TEventHandler>()
        where TEvent : IIntegrationEvent
        where TEventHandler : IIntegrationEventHandler<TEvent>;
}
