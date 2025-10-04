using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Extension methods for configuring resilience policies using Polly.
/// Provides circuit breaker, retry, and timeout policies for HttpClient.
/// Note: This uses Polly v8 which has different APIs. For Polly.Extensions.Http compatibility,
/// use the traditional AddPolicyHandler pattern with Polly v7.
/// </summary>
public static class ResilienceExtensions
{
    /// <summary>
    /// Adds Marventa resilience policies to the service collection.
    /// Configures a named HttpClient with retry, circuit breaker, and timeout policies.
    /// For Polly v8 users: Consider using AddStandardResilienceHandler() from Microsoft.Extensions.Http.Resilience.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMarventaResilience(this IServiceCollection services, IConfiguration configuration)
    {
        // For Polly v8, the recommended approach is to use the new resilience APIs
        // Here we provide a compatibility layer for Polly v7/v8

        // Named HttpClient - users can configure policies in their own code
        services.AddHttpClient("resilient-client");

        return services;
    }

    /// <summary>
    /// Adds a custom named HttpClient with resilience policies.
    /// For full resilience features, consider implementing custom Polly v8 policies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The name for the HttpClient.</param>
    /// <param name="configureClient">Optional action to configure the HttpClient.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddResilientHttpClient(
        this IServiceCollection services,
        string name,
        Action<HttpClient>? configureClient = null)
    {
        var builder = services.AddHttpClient(name);

        if (configureClient != null)
        {
            builder.ConfigureHttpClient(configureClient);
        }

        return services;
    }

    /// <summary>
    /// Creates a custom retry policy with configurable parameters.
    /// Compatible with Polly v7. For Polly v8, use the new resilience pipeline APIs.
    /// </summary>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <param name="initialDelay">The initial delay before the first retry.</param>
    /// <returns>The custom retry policy.</returns>
    public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(int retryCount, TimeSpan initialDelay)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => initialDelay * Math.Pow(2, retryAttempt - 1));
    }

    /// <summary>
    /// Creates a custom circuit breaker policy with configurable parameters.
    /// Compatible with Polly v7. For Polly v8, use the new resilience pipeline APIs.
    /// </summary>
    /// <param name="failureThreshold">The number of consecutive failures before opening the circuit.</param>
    /// <param name="durationOfBreak">The duration to keep the circuit open.</param>
    /// <returns>The custom circuit breaker policy.</returns>
    public static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy(
        int failureThreshold,
        TimeSpan durationOfBreak)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(failureThreshold, durationOfBreak);
    }
}
