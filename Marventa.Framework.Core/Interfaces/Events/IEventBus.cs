namespace Marventa.Framework.Core.Interfaces.Events;

public interface IEventBus
{
    Task PublishDomainEventAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent;
    Task PublishIntegrationEventAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent;
    void Subscribe<T, TH>()
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>;
    void Unsubscribe<T, TH>()
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>;
}