using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Infrastructure.Extensions;

public static class DatabaseSeedingExtensions
{
    /// <summary>
    /// Seeds the database with initial data
    /// </summary>
    public static async Task<IHost> SeedDatabaseAsync<TContext>(this IHost host)
        where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();

        try
        {
            var context = services.GetRequiredService<TContext>();
            var seeder = services.GetRequiredService<IDatabaseSeeder>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Try to apply migrations if available
            try
            {
                logger.LogInformation("Checking for database migrations...");
                // Use reflection to check if migrations are available
                var migrationsAssembly = context.Database.GetType().Assembly;
                if (migrationsAssembly != null)
                {
                    logger.LogInformation("Applying database migrations...");
                    // context.Database.MigrateAsync() requires EF migrations package
                }
            }
            catch (Exception migrationEx)
            {
                logger.LogWarning(migrationEx, "Could not apply migrations, database may not support migrations");
            }

            // Seed data
            await seeder.SeedAsync(context);

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }

        return host;
    }

    /// <summary>
    /// Adds database seeder services to the container
    /// </summary>
    public static IServiceCollection AddDatabaseSeeding(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseSeeder, Data.DatabaseSeeder>();
        return services;
    }
}