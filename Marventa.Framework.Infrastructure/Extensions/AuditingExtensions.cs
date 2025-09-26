using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Marventa.Framework.Infrastructure.Interceptors;

namespace Marventa.Framework.Infrastructure.Extensions;

public static class AuditingExtensions
{
    /// <summary>
    /// Adds Entity Framework auditing support with automatic CreatedBy/UpdatedBy tracking
    /// </summary>
    public static IServiceCollection AddMarventaAuditing(this IServiceCollection services)
    {
        services.AddScoped<AuditingInterceptor>();
        return services;
    }

    /// <summary>
    /// Configures Entity Framework context to use auditing interceptor
    /// </summary>
    public static DbContextOptionsBuilder UseMarventaAuditing(
        this DbContextOptionsBuilder optionsBuilder,
        IServiceProvider serviceProvider)
    {
        var auditingInterceptor = serviceProvider.GetService<AuditingInterceptor>();
        if (auditingInterceptor != null)
        {
            optionsBuilder.AddInterceptors(auditingInterceptor);
        }

        return optionsBuilder;
    }
}