namespace Marventa.Framework.Core.Interfaces.Projections;

public interface IProjectionManager
{
    Task ProjectAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
    Task RebuildProjectionAsync<TProjection>(CancellationToken cancellationToken = default) where TProjection : class, IProjection;
    Task RebuildAllProjectionsAsync(CancellationToken cancellationToken = default);
}