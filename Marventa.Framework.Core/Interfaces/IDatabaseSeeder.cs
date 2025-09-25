namespace Marventa.Framework.Core.Interfaces;

public interface IDatabaseSeeder
{
    Task SeedAsync<TContext>(TContext context) where TContext : class;
}