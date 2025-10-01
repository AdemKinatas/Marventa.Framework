namespace Marventa.Framework.Features.EventBus.Abstractions;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}
