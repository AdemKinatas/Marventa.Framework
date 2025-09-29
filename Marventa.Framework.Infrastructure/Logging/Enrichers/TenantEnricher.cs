using Marventa.Framework.Core.Interfaces.MultiTenancy;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Marventa.Framework.Infrastructure.Logging.Enrichers;

public class TenantEnricher : ILogEventEnricher
{
    private readonly ITenantContext _tenantContext;

    public TenantEnricher(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_tenantContext?.CurrentTenant != null)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "TenantId", _tenantContext.CurrentTenant.Id));

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "TenantName", _tenantContext.CurrentTenant.Name));
        }
        else
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "TenantId", "system"));
        }
    }
}

public static class TenantEnricherExtensions
{
    public static LoggerConfiguration WithTenant(
        this LoggerEnrichmentConfiguration enrichmentConfiguration,
        ITenantContext tenantContext)
    {
        if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
        return enrichmentConfiguration.With(new TenantEnricher(tenantContext));
    }
}