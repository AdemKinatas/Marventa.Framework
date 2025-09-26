using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Marventa.Framework.Infrastructure.Logging.Enrichers;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

namespace Marventa.Framework.Infrastructure.Logging;

public static class SerilogConfiguration
{
    public static IHostBuilder UseMarventaSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) =>
        {
            ConfigureSerilog(configuration, context.Configuration, context.HostingEnvironment);
        });
    }

    public static WebApplicationBuilder AddMarventaSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateBootstrapLogger();

        builder.Host.UseSerilog((context, services, configuration) =>
        {
            ConfigureSerilog(configuration, context.Configuration, context.HostingEnvironment);
        });

        return builder;
    }

    private static void ConfigureSerilog(
        LoggerConfiguration loggerConfiguration,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Application";

        loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", assemblyName)
            .Enrich.WithExceptionDetails();

        // Add correlation ID enricher
        loggerConfiguration.Enrich.WithCorrelationId();

        // Console output
        if (environment.IsDevelopment())
        {
            loggerConfiguration.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}");
        }
        else
        {
            loggerConfiguration.WriteTo.Console(new CompactJsonFormatter());
        }

        // File output
        var logPath = configuration["Serilog:FilePath"] ?? "logs/log-.txt";
        loggerConfiguration.WriteTo.File(
            path: logPath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}",
            shared: true);

        // Elasticsearch output
        var elasticsearchUrl = configuration["Serilog:Elasticsearch:Url"];
        if (!string.IsNullOrWhiteSpace(elasticsearchUrl))
        {
            loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                IndexFormat = $"{assemblyName.ToLower()}-{environment.EnvironmentName.ToLower()}-{{0:yyyy.MM.dd}}",
                NumberOfShards = 2,
                NumberOfReplicas = 1,
                CustomFormatter = new ElasticsearchJsonFormatter(),
                MinimumLogEventLevel = LogEventLevel.Information,
                BatchPostingLimit = 50,
                Period = TimeSpan.FromSeconds(2),
                ConnectionTimeout = TimeSpan.FromSeconds(5),
                FailureCallback = (e, ex) => Console.WriteLine($"Unable to submit event to Elasticsearch: {e.MessageTemplate}"),
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                  EmitEventFailureHandling.WriteToFailureSink |
                                  EmitEventFailureHandling.RaiseCallback
            });
        }

        // Seq output (optional)
        var seqUrl = configuration["Serilog:Seq:Url"];
        if (!string.IsNullOrWhiteSpace(seqUrl))
        {
            var seqApiKey = configuration["Serilog:Seq:ApiKey"];
            loggerConfiguration.WriteTo.Seq(seqUrl, apiKey: seqApiKey);
        }
    }

    public static IApplicationBuilder UseMarventaSerilogRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            // Customize the level based on status code
            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (ex != null || httpContext.Response.StatusCode >= 500)
                    return LogEventLevel.Error;

                if (httpContext.Response.StatusCode >= 400)
                    return LogEventLevel.Warning;

                return LogEventLevel.Information;
            };

            // Attach additional properties
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
                diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);

                // Add user information if authenticated
                if (httpContext.User?.Identity?.IsAuthenticated ?? false)
                {
                    diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
                    diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value);
                }
            };
        });

        return app;
    }
}