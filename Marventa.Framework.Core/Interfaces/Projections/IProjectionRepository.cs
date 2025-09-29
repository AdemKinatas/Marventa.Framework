using System.Linq.Expressions;

namespace Marventa.Framework.Core.Interfaces.Projections;

public interface IProjectionRepository<TProjection> where TProjection : class, IProjection
{
    Task<TProjection?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TProjection>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<TProjection>> FindAsync(Expression<Func<TProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task UpsertAsync(TProjection projection, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
}