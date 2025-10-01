using Microsoft.EntityFrameworkCore;

namespace Marventa.Framework.Infrastructure.Seeding;

public abstract class DataSeederBase<TDbContext> : IDataSeeder
    where TDbContext : BaseDbContext
{
    protected readonly TDbContext Context;

    protected DataSeederBase(TDbContext context)
    {
        Context = context;
    }

    public abstract Task SeedAsync(CancellationToken cancellationToken = default);

    public virtual int Order => 0;

    protected async Task<bool> AnyAsync<TEntity>(CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await Context.Set<TEntity>().AnyAsync(cancellationToken);
    }

    protected async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        await Context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
