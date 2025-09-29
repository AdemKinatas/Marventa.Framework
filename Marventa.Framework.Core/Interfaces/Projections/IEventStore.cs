namespace Marventa.Framework.Core.Interfaces.Projections;

public interface IEventStore
{
    Task<IEnumerable<TEvent>> GetEventsAsync<TEvent>(DateTime? fromTime = null, CancellationToken cancellationToken = default) where TEvent : class;
    Task<IEnumerable<object>> GetAllEventsAsync(DateTime? fromTime = null, CancellationToken cancellationToken = default);
}