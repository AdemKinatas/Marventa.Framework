using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Marventa.Framework.Core.Models;
using Marventa.Framework.Web.Middleware;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// Application builder extensions for middleware configuration
/// </summary>
internal static class ApplicationBuilderExtensions
{
    internal static IApplicationBuilder ConfigureMiddlewarePipeline(
        this IApplicationBuilder app,
        IConfiguration configuration,
        MarventaFrameworkOptions options,
        Microsoft.Extensions.Logging.ILogger logger)
    {
        if (options.MiddlewareOptions.UseUnifiedMiddleware)
        {
            app.UseMiddleware<MarventaUnifiedMiddleware>();
            logger.LogInformation("Marventa Unified Middleware activated (High Performance Mode)");
        }
        else
        {
            app.ConfigureModularMiddleware(options, logger);
        }

        return app.ConfigureStandardPipeline(configuration, options);
    }

    private static IApplicationBuilder ConfigureModularMiddleware(
        this IApplicationBuilder app,
        MarventaFrameworkOptions options,
        Microsoft.Extensions.Logging.ILogger logger)
    {
        // Modular middleware mode is deprecated - use UseUnifiedMiddleware instead
        logger.LogWarning("Modular Middleware mode is deprecated. Please use UseUnifiedMiddleware = true for better performance.");

        app.Use(async (context, next) =>
        {
            AddSecurityHeaders(context);
            await next();
        });

        return app;
    }

    private static IApplicationBuilder ConfigureStandardPipeline(
        this IApplicationBuilder app,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        app.UseHttpsRedirection();

        // Response compression would be used here if enabled
        // if (options.EnableCompression)
        //     app.UseResponseCompression();

        if (options.EnableCaching)
            app.UseResponseCaching();

        app.UseStaticFiles();
        app.UseRouting();

        var corsOrigins = configuration.GetSection("Cors:Origins").Get<string[]>();
        if (corsOrigins?.Length > 0)
            app.UseCors("MarventaPolicy");

        // ApiKeyAuthenticationMiddleware removed - handled by MarventaUnifiedMiddleware

        if (options.EnableJWT || options.EnableApiKeys)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        if (options.EnableLogging)
        {
            var serilogLogger = app.ApplicationServices.GetService<Serilog.ILogger>();
            if (serilogLogger != null)
            {
                app.UseSerilogRequestLogging(opts =>
                {
                    opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                    {
                        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? "unknown");
                        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                        if (!string.IsNullOrEmpty(userAgent))
                        {
                            diagnosticContext.Set("UserAgent", userAgent);
                        }
                    };
                });
            }
        }

        if (options.EnableHealthChecks)
            app.UseHealthChecks("/health");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            if (options.EnableHealthChecks)
                endpoints.MapHealthChecks("/health");

            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Marventa Framework is running!", context.RequestAborted);
            });
        });

        return app;
    }

    private static void AddSecurityHeaders(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["X-XSS-Protection"] = "1; mode=block";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["X-Powered-By"] = "Marventa.Framework";
    }
}
