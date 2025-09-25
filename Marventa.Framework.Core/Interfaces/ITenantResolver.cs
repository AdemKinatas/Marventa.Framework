namespace Marventa.Framework.Core.Interfaces;

public interface ITenantResolver<TContext>
{
    Task<ITenant?> ResolveAsync(TContext context);
}