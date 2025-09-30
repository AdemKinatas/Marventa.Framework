using Marventa.Framework.ExceptionHandling;
using Marventa.Framework.MultiTenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Marventa.Framework.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMarventaFramework(this IApplicationBuilder app, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseMarventaExceptionHandler();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static IApplicationBuilder UseMarventaMultiTenancy(this IApplicationBuilder app)
    {
        app.UseMiddleware<TenantMiddleware>();
        return app;
    }
}
