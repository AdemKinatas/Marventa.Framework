namespace Marventa.Framework.Core.Interfaces;

public interface IProjection
{
    string Id { get; set; }
    DateTime LastUpdated { get; set; }
    string Version { get; set; }
}

public interface IProjectionHandler<TEvent, TProjection>
    where TEvent : class
    where TProjection : class, IProjection
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

public interface IProjectionRepository<TProjection> where TProjection : class, IProjection
{
    Task<TProjection?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TProjection>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<TProjection>> FindAsync(System.Linq.Expressions.Expression<Func<TProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task UpsertAsync(TProjection projection, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
}

public interface IProjectionManager
{
    Task ProjectAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
    Task RebuildProjectionAsync<TProjection>(CancellationToken cancellationToken = default) where TProjection : class, IProjection;
    Task RebuildAllProjectionsAsync(CancellationToken cancellationToken = default);
}

public interface IEventStore
{
    Task<IEnumerable<TEvent>> GetEventsAsync<TEvent>(DateTime? fromTime = null, CancellationToken cancellationToken = default) where TEvent : class;
    Task<IEnumerable<object>> GetAllEventsAsync(DateTime? fromTime = null, CancellationToken cancellationToken = default);
}

public abstract class BaseProjection : IProjection
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string Version { get; set; } = "1.0";
    public string? TenantId { get; set; }

    protected void UpdateVersion()
    {
        var parts = Version.Split('.');
        if (parts.Length >= 2 && int.TryParse(parts[1], out var minor))
        {
            Version = $"{parts[0]}.{minor + 1}";
        }
        LastUpdated = DateTime.UtcNow;
    }
}