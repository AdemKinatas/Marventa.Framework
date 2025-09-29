using Microsoft.AspNetCore.Builder;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// Core framework application builder extensions
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds minimal Marventa Framework middleware (alias for UseMarventaCore)
    /// </summary>
    public static IApplicationBuilder UseMarventaFramework(this IApplicationBuilder app)
    {
        // Basic framework middleware pipeline
        // Users can add more specific middleware using MarventaExtensions
        return app;
    }
}