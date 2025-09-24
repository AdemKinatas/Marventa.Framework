using Marventa.Framework.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework;

public static class MarventaFrameworkExtensions
{
    public static IServiceCollection AddMarventa(this IServiceCollection services)
    {
        return services.AddMarventaFramework();
    }

    public static IApplicationBuilder UseMarventa(this IApplicationBuilder app)
    {
        return app.UseMarventaFramework();
    }
}