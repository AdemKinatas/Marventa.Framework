using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Domain.Entities;

namespace Marventa.Framework.Infrastructure.Projections;

public class SqlServerEventStore : IEventStore
{
    private readonly DbContext _context;
    private readonly ILogger<SqlServerEventStore> _logger;

    public SqlServerEventStore(DbContext context, ILogger<SqlServerEventStore> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TEvent>> GetEventsAsync<TEvent>(DateTime? fromTime = null, CancellationToken cancellationToken = default) where TEvent : class
    {
        try
        {
            var eventTypeName = typeof(TEvent).Name;
            var query = _context.Set<EventStoreEntry>()
                .Where(e => e.EventType == eventTypeName);

            if (fromTime.HasValue)
            {
                query = query.Where(e => e.Timestamp >= fromTime.Value);
            }

            var entries = await query.OrderBy(e => e.Timestamp).ToListAsync(cancellationToken);

            var events = new List<TEvent>();
            foreach (var entry in entries)
            {
                try
                {
                    var eventData = JsonSerializer.Deserialize<TEvent>(entry.EventData);
                    if (eventData != null)
                    {
                        events.Add(eventData);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize event {EventId} of type {EventType}", entry.Id, eventTypeName);
                }
            }

            return events;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events of type {EventType}", typeof(TEvent).Name);
            throw;
        }
    }

    public async Task<IEnumerable<object>> GetAllEventsAsync(DateTime? fromTime = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Set<EventStoreEntry>().AsQueryable();

            if (fromTime.HasValue)
            {
                query = query.Where(e => e.Timestamp >= fromTime.Value);
            }

            var entries = await query.OrderBy(e => e.Timestamp).ToListAsync(cancellationToken);

            var events = new List<object>();
            foreach (var entry in entries)
            {
                try
                {
                    // Try to resolve the actual type
                    var eventType = Type.GetType(entry.EventType) ?? Type.GetType($"{entry.EventType}, {entry.AssemblyName}");

                    if (eventType != null)
                    {
                        var eventData = JsonSerializer.Deserialize(entry.EventData, eventType);
                        if (eventData != null)
                        {
                            events.Add(eventData);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Could not resolve event type {EventType} from assembly {AssemblyName}", entry.EventType, entry.AssemblyName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize event {EventId} of type {EventType}", entry.Id, entry.EventType);
                }
            }

            return events;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all events from event store");
            throw;
        }
    }
}