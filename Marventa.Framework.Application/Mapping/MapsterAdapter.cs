using Mapster;

namespace Marventa.Framework.Application.Mapping;

public class MapsterAdapter : IMapper
{
    public TDestination Map<TDestination>(object source)
    {
        return source.Adapt<TDestination>();
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return source.Adapt<TDestination>();
    }

    public void Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        source.Adapt(destination);
    }

    public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source)
    {
        return source.ProjectToType<TDestination>();
    }
}