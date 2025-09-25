namespace Marventa.Framework.Core.Interfaces;

public interface ISaga
{
    Guid CorrelationId { get; set; }
}

public interface ISagaStateMachine<TInstance> : ISaga
    where TInstance : class, ISaga
{
}

public interface ISagaRepository<TSaga> where TSaga : class, ISaga
{
    Task<TSaga?> GetAsync(Guid correlationId, CancellationToken cancellationToken = default);
    Task AddAsync(TSaga saga, CancellationToken cancellationToken = default);
    Task UpdateAsync(TSaga saga, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid correlationId, CancellationToken cancellationToken = default);
}

public interface ISagaManager
{
    Task<TSaga> StartSagaAsync<TSaga>(object initialEvent, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga, new();

    Task ContinueSagaAsync<TSaga>(Guid correlationId, object @event, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga;

    Task CompensateAsync<TSaga>(Guid correlationId, string reason, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga;
}

public interface ISagaOrchestrator<TSaga> where TSaga : class, ISaga
{
    Task HandleAsync(TSaga saga, object @event, CancellationToken cancellationToken = default);
    Task CompensateAsync(TSaga saga, string reason, CancellationToken cancellationToken = default);
}