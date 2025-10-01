namespace Marventa.Framework.Features.Caching.Abstractions;

public class CacheOptions
{
    public TimeSpan? AbsoluteExpiration { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }

    public static CacheOptions DefaultExpiration => new()
    {
        AbsoluteExpiration = TimeSpan.FromMinutes(30)
    };

    public static CacheOptions NoExpiration => new();

    public static CacheOptions WithAbsoluteExpiration(TimeSpan expiration) => new()
    {
        AbsoluteExpiration = expiration
    };

    public static CacheOptions WithSlidingExpiration(TimeSpan expiration) => new()
    {
        SlidingExpiration = expiration
    };
}
