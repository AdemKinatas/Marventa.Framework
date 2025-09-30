using Marventa.Framework.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Infrastructure;

public static class EfCoreExtensions
{
    public static IServiceCollection AddMarventaDbContext<TContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(optionsAction);
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    public static IServiceCollection AddMarventaRepository<TEntity, TId, TRepository>(
        this IServiceCollection services)
        where TEntity : Entity<TId>
        where TRepository : class, IRepository<TEntity, TId>
    {
        services.AddScoped<IRepository<TEntity, TId>, TRepository>();
        return services;
    }

    public static IServiceCollection AddMarventaGenericRepository<TEntity, TId>(
        this IServiceCollection services)
        where TEntity : Entity<TId>
    {
        services.AddScoped<IRepository<TEntity, TId>, GenericRepository<TEntity, TId>>();
        return services;
    }
}
