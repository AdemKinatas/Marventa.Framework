using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Domain.Entities;

namespace Marventa.Framework.Infrastructure.Sagas;

public class SimpleSagaRepository<TSaga> : ISagaRepository<TSaga> where TSaga : class, ISaga
{
    private readonly DbContext _context;
    private readonly ILogger<SimpleSagaRepository<TSaga>> _logger;
    private readonly ITenantContext _tenantContext;

    public SimpleSagaRepository(
        DbContext context,
        ILogger<SimpleSagaRepository<TSaga>> logger,
        ITenantContext tenantContext)
    {
        _context = context;
        _logger = logger;
        _tenantContext = tenantContext;
    }

    public async Task<TSaga?> GetAsync(Guid correlationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var sagaType = typeof(TSaga).Name;
            var sagaState = await _context.Set<SagaState>()
                .FirstOrDefaultAsync(s => s.CorrelationId == correlationId && s.SagaType == sagaType, cancellationToken);

            if (sagaState == null)
                return null;

            var sagaData = sagaState.GetData<TSaga>();
            if (sagaData != null)
            {
                sagaData.CorrelationId = sagaState.CorrelationId;
            }

            return sagaData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, correlationId);
            throw;
        }
    }

    public async Task AddAsync(TSaga saga, CancellationToken cancellationToken = default)
    {
        try
        {
            var sagaState = new SagaState
            {
                CorrelationId = saga.CorrelationId,
                SagaType = typeof(TSaga).Name,
                Status = SagaStatus.Started,
                TenantId = _tenantContext.TenantId
            };

            sagaState.SetData(saga);

            _context.Set<SagaState>().Add(sagaState);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Added saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, saga.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, saga.CorrelationId);
            throw;
        }
    }

    public async Task UpdateAsync(TSaga saga, CancellationToken cancellationToken = default)
    {
        try
        {
            var sagaType = typeof(TSaga).Name;
            var sagaState = await _context.Set<SagaState>()
                .FirstOrDefaultAsync(s => s.CorrelationId == saga.CorrelationId && s.SagaType == sagaType, cancellationToken);

            if (sagaState == null)
            {
                await AddAsync(saga, cancellationToken);
                return;
            }

            sagaState.SetData(saga);

            if (saga is BaseSagaState baseSaga)
            {
                sagaState.Status = baseSaga.Status;
                sagaState.CurrentStep = baseSaga.CurrentStep;
                sagaState.ErrorMessage = baseSaga.ErrorMessage;
                sagaState.CompletedSteps = baseSaga.CompletedSteps;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Updated saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, saga.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, saga.CorrelationId);
            throw;
        }
    }

    public async Task DeleteAsync(Guid correlationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var sagaType = typeof(TSaga).Name;
            var sagaState = await _context.Set<SagaState>()
                .FirstOrDefaultAsync(s => s.CorrelationId == correlationId && s.SagaType == sagaType, cancellationToken);

            if (sagaState != null)
            {
                _context.Set<SagaState>().Remove(sagaState);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogDebug("Deleted saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, correlationId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting saga {SagaType} with correlation ID {CorrelationId}", typeof(TSaga).Name, correlationId);
            throw;
        }
    }
}