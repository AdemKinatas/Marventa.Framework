using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Marventa.Framework.Application.Extensions;

public static class MappingExtensions
{
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<MapsterMapper.IMapper, MapsterMapper.Mapper>();
        services.AddScoped<Mapping.IMapper, Mapping.MapsterAdapter>();

        return services;
    }

    public static TypeAdapterConfig ConfigureMapping(this TypeAdapterConfig config)
    {
        config.NewConfig<object, object>()
            .IgnoreNullValues(true)
            .PreserveReference(true);

        return config;
    }
}