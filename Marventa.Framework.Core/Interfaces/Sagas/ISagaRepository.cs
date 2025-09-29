namespace Marventa.Framework.Core.Interfaces.Sagas;

public interface ISagaRepository<TSaga> where TSaga : class, ISaga
{
    Task<TSaga?> GetAsync(Guid correlationId, CancellationToken cancellationToken = default);
    Task AddAsync(TSaga saga, CancellationToken cancellationToken = default);
    Task UpdateAsync(TSaga saga, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid correlationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TSaga>> GetByStatusAsync(SagaStatus status, CancellationToken cancellationToken = default);
}