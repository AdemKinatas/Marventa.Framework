namespace Marventa.Framework.Application.Mapping;

public interface IMapper
{
    TDestination Map<TDestination>(object source);
    TDestination Map<TSource, TDestination>(TSource source);
    void Map<TSource, TDestination>(TSource source, TDestination destination);
    IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source);
}