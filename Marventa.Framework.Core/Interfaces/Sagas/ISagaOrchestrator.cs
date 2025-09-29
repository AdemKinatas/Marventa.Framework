namespace Marventa.Framework.Core.Interfaces.Sagas;

public interface ISagaOrchestrator<TSaga> where TSaga : class, ISaga
{
    Task HandleAsync(TSaga saga, object @event, CancellationToken cancellationToken = default);
    Task CompensateAsync(TSaga saga, string reason, CancellationToken cancellationToken = default);
}