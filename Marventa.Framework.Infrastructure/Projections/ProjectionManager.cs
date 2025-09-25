using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Infrastructure.Projections;

public class ProjectionManager : IProjectionManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProjectionManager> _logger;

    public ProjectionManager(IServiceProvider serviceProvider, ILogger<ProjectionManager> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ProjectAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        try
        {
            var handlers = _serviceProvider.GetServices<IProjectionHandler<TEvent, IProjection>>();
            var tasks = handlers.Select(handler => handler.HandleAsync(@event, cancellationToken));

            await Task.WhenAll(tasks);

            _logger.LogDebug("Projected event {EventType} to {HandlerCount} projections", typeof(TEvent).Name, handlers.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error projecting event {EventType}", typeof(TEvent).Name);
            throw;
        }
    }

    public async Task RebuildProjectionAsync<TProjection>(CancellationToken cancellationToken = default) where TProjection : class, IProjection
    {
        try
        {
            _logger.LogInformation("Starting rebuild of projection {ProjectionType}", typeof(TProjection).Name);

            var repository = _serviceProvider.GetRequiredService<IProjectionRepository<TProjection>>();
            var eventStore = _serviceProvider.GetService<IEventStore>();

            if (eventStore == null)
            {
                _logger.LogWarning("No event store configured, cannot rebuild projection {ProjectionType}", typeof(TProjection).Name);
                return;
            }

            // Clear existing projections (optional - could be made configurable)
            await ClearProjectionAsync<TProjection>(repository, cancellationToken);

            // Get all events from event store
            var allEvents = await eventStore.GetAllEventsAsync(cancellationToken: cancellationToken);

            var processedCount = 0;
            foreach (var eventObj in allEvents)
            {
                await ProjectEventToSpecificProjection<TProjection>(eventObj, cancellationToken);
                processedCount++;

                if (processedCount % 1000 == 0)
                {
                    _logger.LogInformation("Processed {Count} events for projection {ProjectionType}", processedCount, typeof(TProjection).Name);
                }
            }

            _logger.LogInformation("Completed rebuild of projection {ProjectionType}, processed {Count} events", typeof(TProjection).Name, processedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rebuilding projection {ProjectionType}", typeof(TProjection).Name);
            throw;
        }
    }

    public async Task RebuildAllProjectionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting rebuild of all projections");

            var eventStore = _serviceProvider.GetService<IEventStore>();
            if (eventStore == null)
            {
                _logger.LogWarning("No event store configured, cannot rebuild projections");
                return;
            }

            // Get all registered projection types
            var projectionTypes = GetRegisteredProjectionTypes();

            foreach (var projectionType in projectionTypes)
            {
                var method = GetType().GetMethod(nameof(RebuildProjectionAsync))?.MakeGenericMethod(projectionType);
                if (method != null)
                {
                    await (Task)method.Invoke(this, new object[] { cancellationToken })!;
                }
            }

            _logger.LogInformation("Completed rebuild of all projections");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rebuilding all projections");
            throw;
        }
    }

    private async Task ProjectEventToSpecificProjection<TProjection>(object eventObj, CancellationToken cancellationToken)
        where TProjection : class, IProjection
    {
        var eventType = eventObj.GetType();
        var handlerType = typeof(IProjectionHandler<,>).MakeGenericType(eventType, typeof(TProjection));
        var handler = _serviceProvider.GetService(handlerType);

        if (handler != null)
        {
            var method = handlerType.GetMethod("HandleAsync");
            if (method != null)
            {
                await (Task)method.Invoke(handler, new[] { eventObj, cancellationToken })!;
            }
        }
    }

    private async Task ClearProjectionAsync<TProjection>(IProjectionRepository<TProjection> repository, CancellationToken cancellationToken)
        where TProjection : class, IProjection
    {
        // This is a simplified approach - in production you might want to be more careful
        var allProjections = await repository.GetAllAsync(0, int.MaxValue, cancellationToken);
        foreach (var projection in allProjections)
        {
            await repository.DeleteAsync(projection.Id, cancellationToken);
        }
    }

    private Type[] GetRegisteredProjectionTypes()
    {
        // This is a simplified approach - you might want to use reflection or maintain a registry
        var repositoryServices = _serviceProvider.GetServices<object>()
            .Where(s => s.GetType().IsGenericType &&
                       s.GetType().GetGenericTypeDefinition() == typeof(IProjectionRepository<>))
            .ToList();

        return repositoryServices
            .Select(s => s.GetType().GetGenericArguments().First())
            .Where(t => t.IsAssignableTo(typeof(IProjection)))
            .ToArray();
    }
}