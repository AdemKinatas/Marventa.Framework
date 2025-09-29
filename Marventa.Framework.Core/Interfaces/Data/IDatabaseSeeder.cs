namespace Marventa.Framework.Core.Interfaces.Data;

public interface IDatabaseSeeder
{
    Task SeedAsync<TContext>(TContext context) where TContext : class;
}