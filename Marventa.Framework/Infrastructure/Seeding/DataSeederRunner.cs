using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Seeding;

public class DataSeederRunner
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataSeederRunner> _logger;

    public DataSeederRunner(IServiceProvider serviceProvider, ILogger<DataSeederRunner> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>()
            .OrderBy(s => s.Order)
            .ToList();

        if (!seeders.Any())
        {
            _logger.LogInformation("No data seeders found");
            return;
        }

        _logger.LogInformation("Starting data seeding with {Count} seeders", seeders.Count);

        foreach (var seeder in seeders)
        {
            try
            {
                _logger.LogInformation("Running seeder: {SeederName}", seeder.GetType().Name);
                await seeder.SeedAsync(cancellationToken);
                _logger.LogInformation("Seeder completed: {SeederName}", seeder.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running seeder: {SeederName}", seeder.GetType().Name);
                throw;
            }
        }

        _logger.LogInformation("Data seeding completed successfully");
    }
}
