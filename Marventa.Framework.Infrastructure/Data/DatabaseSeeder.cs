using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Marventa.Framework.Core.Interfaces.Data;
using Marventa.Framework.Domain.Entities;

namespace Marventa.Framework.Infrastructure.Data;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ILogger<DatabaseSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync<TContext>(TContext context) where TContext : class
    {
        if (context is not DbContext dbContext)
        {
            _logger.LogWarning("Context is not a DbContext, skipping seeding");
            return;
        }

        _logger.LogInformation("Starting database seeding");

        try
        {
            await SeedTenantsAsync(dbContext);
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding");
            throw;
        }
    }

    private async Task SeedTenantsAsync(DbContext context)
    {
        try
        {
            // Try to seed tenants if Tenant entity exists
            var tenantEntityType = context.Model.GetEntityTypes()
                .FirstOrDefault(t => t.ClrType == typeof(Tenant));

            if (tenantEntityType != null)
            {
                var tenantSet = context.Set<Tenant>();

                if (!await tenantSet.AnyAsync())
                {
                    _logger.LogInformation("Seeding default tenants");

                    var defaultTenants = new[]
                    {
                        new Tenant
                        {
                            Id = Guid.NewGuid(),
                            Name = "Demo Company",
                            ConnectionString = "Server=localhost;Database=Demo;Trusted_Connection=true;",
                            Properties = new Dictionary<string, object>
                            {
                                ["CompanyType"] = "Demo",
                                ["MaxUsers"] = 100
                            }
                        },
                        new Tenant
                        {
                            Id = Guid.NewGuid(),
                            Name = "Enterprise Corp",
                            ConnectionString = "Server=localhost;Database=Enterprise;Trusted_Connection=true;",
                            Properties = new Dictionary<string, object>
                            {
                                ["CompanyType"] = "Enterprise",
                                ["MaxUsers"] = 1000
                            }
                        }
                    };

                    tenantSet.AddRange(defaultTenants);
                    await context.SaveChangesAsync();

                    _logger.LogInformation("Seeded {Count} tenants", defaultTenants.Length);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to seed tenants");
        }
    }
}