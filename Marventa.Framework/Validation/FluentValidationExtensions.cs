using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Marventa.Framework.Validation;

public static class FluentValidationExtensions
{
    public static IServiceCollection AddMarventaValidation(this IServiceCollection services, Assembly assembly)
    {
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}
