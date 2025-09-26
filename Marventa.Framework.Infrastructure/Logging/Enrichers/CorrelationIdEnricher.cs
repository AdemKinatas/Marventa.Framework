using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Marventa.Framework.Infrastructure.Logging.Enrichers;

public class CorrelationIdEnricher : ILogEventEnricher
{
    private const string CorrelationIdPropertyName = "CorrelationId";
    private readonly string _correlationId;

    public CorrelationIdEnricher() : this(Guid.NewGuid().ToString())
    {
    }

    public CorrelationIdEnricher(string correlationId)
    {
        _correlationId = correlationId;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            CorrelationIdPropertyName, _correlationId));
    }
}

public static class CorrelationIdEnricherExtensions
{
    public static LoggerConfiguration WithCorrelationId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
        return enrichmentConfiguration.With<CorrelationIdEnricher>();
    }

    public static LoggerConfiguration WithCorrelationId(
        this LoggerEnrichmentConfiguration enrichmentConfiguration,
        string correlationId)
    {
        if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
        return enrichmentConfiguration.With(new CorrelationIdEnricher(correlationId));
    }
}