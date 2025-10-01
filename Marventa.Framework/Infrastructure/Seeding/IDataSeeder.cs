namespace Marventa.Framework.Infrastructure.Seeding;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
    int Order { get; }
}
