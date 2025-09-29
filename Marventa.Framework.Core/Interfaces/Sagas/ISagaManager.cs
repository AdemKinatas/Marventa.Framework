namespace Marventa.Framework.Core.Interfaces.Sagas;

public interface ISagaManager
{
    Task<TSaga> StartSagaAsync<TSaga>(object initialEvent, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga, new();

    Task ContinueSagaAsync<TSaga>(Guid correlationId, object @event, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga;

    Task CompensateAsync<TSaga>(Guid correlationId, string reason, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga;

    Task<SagaStatus?> GetSagaStatusAsync<TSaga>(Guid correlationId, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga;

    Task CompleteSagaAsync<TSaga>(Guid correlationId, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga;
}