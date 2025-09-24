using Marventa.Framework.Web.Middleware;
using Marventa.Framework.Web.RateLimiting;
using Marventa.Framework.Web.Security;
using Marventa.Framework.Web.Versioning;
using Microsoft.AspNetCore.Builder;

namespace Marventa.Framework.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMarventaFramework(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseMarventaApiVersioning(this IApplicationBuilder app)
    {
        app.UseMiddleware<VersioningMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseMarventaRateLimiting(this IApplicationBuilder app)
    {
        app.UseMiddleware<RateLimitingMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseMarventaJwtAuthentication(this IApplicationBuilder app)
    {
        app.UseMiddleware<JwtAuthenticationMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseMarventaApiKey(this IApplicationBuilder app)
    {
        app.UseMiddleware<ApiKeyMiddleware>();
        return app;
    }
}