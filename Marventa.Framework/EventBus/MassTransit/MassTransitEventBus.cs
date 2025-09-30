using MassTransit;
using Marventa.Framework.EventBus.Abstractions;

namespace Marventa.Framework.EventBus.MassTransit;

public class MassTransitEventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventBus(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        await _publishEndpoint.Publish(@event, cancellationToken);
    }

    public void Subscribe<TEvent, TEventHandler>()
        where TEvent : IIntegrationEvent
        where TEventHandler : IIntegrationEventHandler<TEvent>
    {
        // MassTransit handles subscriptions through consumer configuration
        // This is typically done in the MassTransitConfiguration class
    }
}
