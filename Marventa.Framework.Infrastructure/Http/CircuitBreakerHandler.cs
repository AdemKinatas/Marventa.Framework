using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Http;

public class CircuitBreakerHandler : DelegatingHandler
{
    private readonly ILogger<CircuitBreakerHandler> _logger;
    private readonly int _failureThreshold;
    private readonly TimeSpan _timeout;
    private int _failureCount;
    private DateTime _lastFailureTime;
    private CircuitState _state = CircuitState.Closed;

    public CircuitBreakerHandler(ILogger<CircuitBreakerHandler> logger, int failureThreshold = 5, int timeoutSeconds = 60)
    {
        _logger = logger;
        _failureThreshold = failureThreshold;
        _timeout = TimeSpan.FromSeconds(timeoutSeconds);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_state == CircuitState.Open)
        {
            if (DateTime.UtcNow - _lastFailureTime >= _timeout)
            {
                _state = CircuitState.HalfOpen;
                _logger.LogInformation("Circuit breaker moved to Half-Open state");
            }
            else
            {
                _logger.LogWarning("Circuit breaker is Open - rejecting request");
                throw new CircuitBreakerOpenException("Circuit breaker is open");
            }
        }

        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                if (_state == CircuitState.HalfOpen)
                {
                    _state = CircuitState.Closed;
                    _failureCount = 0;
                    _logger.LogInformation("Circuit breaker moved to Closed state");
                }
                return response;
            }
            else
            {
                OnFailure();
                return response;
            }
        }
        catch (Exception)
        {
            OnFailure();
            throw;
        }
    }

    private void OnFailure()
    {
        _failureCount++;
        _lastFailureTime = DateTime.UtcNow;

        if (_failureCount >= _failureThreshold)
        {
            _state = CircuitState.Open;
            _logger.LogWarning("Circuit breaker opened due to {FailureCount} failures", _failureCount);
        }
    }
}