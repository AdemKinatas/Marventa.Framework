using Microsoft.AspNetCore.Builder;

namespace Marventa.Framework.ExceptionHandling;

public static class GlobalExceptionHandler
{
    public static IApplicationBuilder UseMarventaExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
}
