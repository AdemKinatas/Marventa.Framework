using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Marventa.Framework.Logging;

public static class SerilogConfiguration
{
    public static ILogger ConfigureSerilog(IConfiguration configuration, string applicationName)
    {
        var elasticsearchUri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
        var environment = configuration["Environment"] ?? "Development";

        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUri))
            {
                IndexFormat = $"{applicationName.ToLower()}-logs-{DateTime.UtcNow:yyyy-MM}",
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                MinimumLogEventLevel = LogEventLevel.Information
            })
            .CreateLogger();
    }
}
