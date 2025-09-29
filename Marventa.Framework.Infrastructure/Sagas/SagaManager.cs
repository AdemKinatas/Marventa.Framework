using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces.Sagas;

namespace Marventa.Framework.Infrastructure.Sagas;

public class SagaManager : ISagaManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SagaManager> _logger;

    public SagaManager(IServiceProvider serviceProvider, ILogger<SagaManager> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<TSaga> StartSagaAsync<TSaga>(object initialEvent, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga, new()
    {
        try
        {
            var saga = new TSaga();
            var repository = _serviceProvider.GetRequiredService<ISagaRepository<TSaga>>();
            var orchestrator = _serviceProvider.GetService<ISagaOrchestrator<TSaga>>();

            await repository.AddAsync(saga, cancellationToken);

            if (orchestrator != null)
            {
                await orchestrator.HandleAsync(saga, initialEvent, cancellationToken);
                await repository.UpdateAsync(saga, cancellationToken);
            }

            _logger.LogInformation("Started saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, saga.CorrelationId);
            return saga;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting saga {SagaType}", typeof(TSaga).Name);
            throw;
        }
    }

    public async Task ContinueSagaAsync<TSaga>(Guid correlationId, object @event, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga
    {
        try
        {
            var repository = _serviceProvider.GetRequiredService<ISagaRepository<TSaga>>();
            var orchestrator = _serviceProvider.GetService<ISagaOrchestrator<TSaga>>();

            var saga = await repository.GetAsync(correlationId, cancellationToken);
            if (saga == null)
            {
                _logger.LogWarning("Saga {SagaType} with correlation ID {CorrelationId} not found", typeof(TSaga).Name, correlationId);
                return;
            }

            if (orchestrator != null)
            {
                await orchestrator.HandleAsync(saga, @event, cancellationToken);
                await repository.UpdateAsync(saga, cancellationToken);
            }

            _logger.LogDebug("Continued saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error continuing saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, correlationId);
            throw;
        }
    }

    public async Task CompensateAsync<TSaga>(Guid correlationId, string reason, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga
    {
        try
        {
            var repository = _serviceProvider.GetRequiredService<ISagaRepository<TSaga>>();
            var orchestrator = _serviceProvider.GetService<ISagaOrchestrator<TSaga>>();

            var saga = await repository.GetAsync(correlationId, cancellationToken);
            if (saga == null)
            {
                _logger.LogWarning("Saga {SagaType} with correlation ID {CorrelationId} not found for compensation", typeof(TSaga).Name, correlationId);
                return;
            }

            if (orchestrator != null)
            {
                await orchestrator.CompensateAsync(saga, reason, cancellationToken);
                await repository.UpdateAsync(saga, cancellationToken);
            }

            _logger.LogInformation("Compensated saga {SagaType} with correlation ID {CorrelationId}, reason: {Reason}", typeof(TSaga).Name, correlationId, reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error compensating saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, correlationId);
            throw;
        }
    }

    public async Task<SagaStatus?> GetSagaStatusAsync<TSaga>(Guid correlationId, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga
    {
        try
        {
            var repository = _serviceProvider.GetRequiredService<ISagaRepository<TSaga>>();
            var saga = await repository.GetAsync(correlationId, cancellationToken);
            return saga?.Status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting saga {SagaType} status for correlation ID {CorrelationId}", typeof(TSaga).Name, correlationId);
            throw;
        }
    }

    public async Task CompleteSagaAsync<TSaga>(Guid correlationId, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga
    {
        try
        {
            var repository = _serviceProvider.GetRequiredService<ISagaRepository<TSaga>>();
            var saga = await repository.GetAsync(correlationId, cancellationToken);
            if (saga == null)
            {
                _logger.LogWarning("Saga {SagaType} with correlation ID {CorrelationId} not found for completion", typeof(TSaga).Name, correlationId);
                return;
            }

            saga.Status = SagaStatus.Completed;
            saga.CompletedAt = DateTime.UtcNow;
            await repository.UpdateAsync(saga, cancellationToken);

            _logger.LogInformation("Completed saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, correlationId);
            throw;
        }
    }
}