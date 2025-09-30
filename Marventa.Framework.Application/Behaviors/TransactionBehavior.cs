using MediatR;
using Marventa.Framework.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior for automatic transaction management
/// Wraps command handlers in a database transaction
/// Commits on success, rolls back on exception
/// Used by all services to ensure data consistency
/// </summary>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Only apply transactions to commands (not queries)
        if (!requestName.EndsWith("Command"))
        {
            return await next();
        }

        _logger.LogInformation("Starting transaction for {RequestName}", requestName);

        try
        {
            var response = await next();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Transaction committed for {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}. Rolling back.", requestName);
            throw;
        }
    }
}