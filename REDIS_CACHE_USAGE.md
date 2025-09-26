# Redis Cache Implementation Guide

## ✅ Implementation Complete!

Redis Cache Service has been successfully added to Marventa.Framework with:

### Features Implemented
- ✅ **RedisCacheService** - Full Redis cache implementation
- ✅ **DistributedCacheService** - Microsoft.Extensions.Caching wrapper
- ✅ **Multi-tenant support** - Automatic key isolation per tenant
- ✅ **Configuration options** - Flexible Redis settings
- ✅ **Service registration** - Easy DI setup
- ✅ **Error handling** - Configurable error behavior
- ✅ **Cache patterns** - GetOrSet, SetIfNotExists, Increment

## Usage Examples

### 1. Configuration in appsettings.json

```json
{
  "Redis": {
    "ConnectionString": "localhost:6379,password=yourpassword",
    "Database": 0,
    "KeyPrefix": "myapp",
    "DefaultExpiration": "00:05:00",
    "EnableMultiTenancy": true,
    "ThrowOnError": false,
    "AllowFlushDatabase": false
  }
}
```

### 2. Register in Program.cs

```csharp
// Option 1: Use Redis directly
builder.Services.AddRedisCache(builder.Configuration);

// Option 2: Use with custom options
builder.Services.AddRedisCache(new RedisCacheOptions
{
    ConnectionString = "localhost:6379",
    DefaultExpiration = TimeSpan.FromMinutes(10),
    EnableMultiTenancy = true,
    KeyPrefix = "myapp"
});

// Option 3: Use Microsoft's IDistributedCache
builder.Services.AddDistributedCache(services =>
{
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost:6379";
    });
});
```

### 3. Use in Services

```csharp
public class ProductService
{
    private readonly ICacheService _cacheService;
    private readonly IProductRepository _repository;

    public ProductService(ICacheService cacheService, IProductRepository repository)
    {
        _cacheService = cacheService;
        _repository = repository;
    }

    // Basic Get/Set
    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";

        // Try to get from cache
        var cached = await _cacheService.GetAsync<Product>(cacheKey);
        if (cached != null)
            return cached;

        // Get from database
        var product = await _repository.GetByIdAsync(id);

        // Store in cache for 5 minutes
        if (product != null)
        {
            await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(5));
        }

        return product;
    }

    // Using GetOrSet pattern
    public async Task<Product?> GetProductOptimizedAsync(int id)
    {
        var cacheKey = $"product:{id}";

        return await _cacheService.GetOrSetAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id),
            TimeSpan.FromMinutes(5)
        );
    }

    // Invalidate cache
    public async Task UpdateProductAsync(Product product)
    {
        await _repository.UpdateAsync(product);

        // Remove from cache
        await _cacheService.RemoveAsync($"product:{product.Id}");

        // Remove related caches
        await _cacheService.RemoveByPatternAsync($"products:category:{product.CategoryId}:*");
    }

    // Using counters
    public async Task<long> IncrementViewCountAsync(int productId)
    {
        var cacheKey = $"product:views:{productId}";
        return await _cacheService.IncrementAsync(cacheKey);
    }

    // Conditional caching
    public async Task<bool> TryReserveProductAsync(int productId, int userId)
    {
        var cacheKey = $"product:reserved:{productId}";

        // Set only if not exists (first user wins)
        return await _cacheService.SetIfNotExistsAsync(
            cacheKey,
            userId,
            TimeSpan.FromMinutes(15)  // 15 minute reservation
        );
    }
}
```

### 4. Multi-Tenant Usage

```csharp
public class TenantAwareService
{
    private readonly ICacheService _cacheService;
    private readonly ITenantContext _tenantContext;

    // Cache keys are automatically scoped to tenant
    public async Task<Settings> GetTenantSettingsAsync()
    {
        // Key will be: "myapp:tenant-123:settings"
        return await _cacheService.GetOrSetAsync(
            "settings",
            async () => await LoadSettingsFromDatabase(),
            TimeSpan.FromHours(1)
        );
    }

    // Clear all tenant cache
    public async Task ClearTenantCacheAsync()
    {
        // Only clears current tenant's cache
        await _cacheService.ClearAsync();
    }
}
```

## Cache Key Structure

### With Multi-Tenancy Enabled
```
{prefix}:{tenantId}:{key}
Example: myapp:tenant-123:product:456
```

### Without Multi-Tenancy
```
{prefix}:{key}
Example: myapp:product:456
```

## Performance Tips

### 1. Use Appropriate Expiration Times
```csharp
// Frequently changing data
TimeSpan.FromSeconds(30)

// Normal data
TimeSpan.FromMinutes(5)

// Static/reference data
TimeSpan.FromHours(24)
```

### 2. Batch Operations
```csharp
// Instead of multiple calls
var product = await cache.GetAsync<Product>($"product:{id}");
var reviews = await cache.GetAsync<List<Review>>($"reviews:{id}");

// Consider combining
var productData = await cache.GetOrSetAsync(
    $"product-full:{id}",
    async () => new {
        Product = await GetProduct(id),
        Reviews = await GetReviews(id)
    },
    TimeSpan.FromMinutes(5)
);
```

### 3. Use Patterns for Invalidation
```csharp
// When category changes, invalidate all products in category
await cache.RemoveByPatternAsync($"products:category:{categoryId}:*");
```

## Error Handling

### ThrowOnError = false (Default)
```csharp
// Cache errors are logged but don't break the application
var cached = await cache.GetAsync<Product>(key);
// Returns null if Redis is down
```

### ThrowOnError = true
```csharp
try
{
    var cached = await cache.GetAsync<Product>(key);
}
catch (RedisConnectionException ex)
{
    // Handle Redis connection issues
}
```

## Monitoring & Debugging

### Check Redis Connection
```csharp
public class HealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.PingAsync();
            return HealthCheckResult.Healthy("Redis is responsive");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis is not responding", ex);
        }
    }
}
```

### View Cache Keys (Debug Only)
```csharp
// WARNING: Use only in development!
var server = _redis.GetServer(_redis.GetEndPoints()[0]);
var keys = server.Keys(pattern: "myapp:*").ToList();
```

## Migration from MemoryCache

Simply change the registration:

```csharp
// From:
services.AddMemoryCache();

// To:
services.AddRedisCache(configuration);
```

No code changes required in your services! The ICacheService interface remains the same.

## Next Steps

Now that Redis Cache is implemented, consider:

1. **Add cache warming** - Pre-load frequently accessed data
2. **Implement cache-aside pattern** - For complex scenarios
3. **Add metrics** - Track hit/miss ratios
4. **Set up Redis Sentinel** - For high availability
5. **Configure Redis Cluster** - For horizontal scaling