using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Marventa.Framework.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior for logging and performance monitoring
/// Automatically logs all requests with execution time
/// Warns about long-running operations (>500ms)
/// Used by all services for consistent logging and performance tracking
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling {RequestName}", requestName);

        try
        {
            var response = await next();

            stopwatch.Stop();
            _logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds}ms",
                requestName, stopwatch.ElapsedMilliseconds);

            // Warn about long running requests
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning("Long running request: {RequestName} took {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error handling {RequestName} after {ElapsedMilliseconds}ms",
                requestName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}