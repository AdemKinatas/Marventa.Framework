namespace Marventa.Framework.Core.Interfaces.Projections;

public interface IProjectionHandler<TEvent, TProjection>
    where TEvent : class
    where TProjection : class, IProjection
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}