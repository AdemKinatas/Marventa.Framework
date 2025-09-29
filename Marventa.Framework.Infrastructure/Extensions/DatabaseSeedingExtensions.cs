using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces.Data;

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

            try
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    logger.LogInformation("Applying database migrations...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Database migrations applied successfully");
                }
            }
            catch (Exception migrationEx)
            {
                logger.LogWarning(migrationEx, "Could not apply migrations, falling back to EnsureCreated");
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