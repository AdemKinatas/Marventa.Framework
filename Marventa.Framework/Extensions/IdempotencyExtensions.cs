using Marventa.Framework.Features.Idempotency;
using Marventa.Framework.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Extension methods for configuring idempotency support.
/// </summary>
public static class IdempotencyExtensions
{
    /// <summary>
    /// Adds idempotency services to the service collection.
    /// Requires distributed cache to be configured.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddIdempotency(this IServiceCollection services)
    {
        services.AddScoped<IIdempotencyService, IdempotencyService>();
        return services;
    }

    /// <summary>
    /// Adds the idempotency middleware to the application pipeline.
    /// This middleware must be added after routing and before endpoints.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseIdempotency(this IApplicationBuilder app)
    {
        return app.UseMiddleware<IdempotencyMiddleware>();
    }
}
