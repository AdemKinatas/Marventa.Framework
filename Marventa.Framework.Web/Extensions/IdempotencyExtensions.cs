using Microsoft.Extensions.DependencyInjection;
using Marventa.Framework.Web.Filters;

namespace Marventa.Framework.Web.Extensions;

public static class IdempotencyExtensions
{
    /// <summary>
    /// Adds idempotency filter for Controllers
    /// </summary>
    public static IServiceCollection AddIdempotencyFilter(this IServiceCollection services)
    {
        services.AddScoped<IdempotencyFilter>();

        services.AddControllers(options =>
        {
            options.Filters.AddService<IdempotencyFilter>();
        });

        return services;
    }

    /// <summary>
    /// Adds global idempotency filter for all controllers
    /// </summary>
    public static IServiceCollection AddGlobalIdempotency(this IServiceCollection services)
    {
        services.AddScoped<IdempotencyFilter>();

        services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
        {
            options.Filters.AddService<IdempotencyFilter>();
        });

        return services;
    }
}