using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Marventa.Framework.CQRS;

public static class MediatorExtensions
{
    public static IServiceCollection AddMarventaMediatR(this IServiceCollection services, Assembly assembly)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        return services;
    }
}
