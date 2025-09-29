namespace Marventa.Framework.Core.Interfaces.MultiTenancy;

public interface ITenantResolver<TContext>
{
    Task<ITenant?> ResolveAsync(TContext context);
}