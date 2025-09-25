using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Marventa.Framework.Core.Interfaces;
using System.Linq.Expressions;

namespace Marventa.Framework.Infrastructure.Projections;

public class MongoProjectionRepository<TProjection> : IProjectionRepository<TProjection>
    where TProjection : class, IProjection
{
    private readonly IMongoCollection<TProjection> _collection;
    private readonly ILogger<MongoProjectionRepository<TProjection>> _logger;
    private readonly ITenantContext _tenantContext;

    public MongoProjectionRepository(
        IMongoDatabase database,
        ILogger<MongoProjectionRepository<TProjection>> logger,
        ITenantContext tenantContext,
        IOptions<MongoProjectionOptions> options)
    {
        _logger = logger;
        _tenantContext = tenantContext;

        var collectionName = options.Value.GetCollectionName<TProjection>();
        _collection = database.GetCollection<TProjection>(collectionName);

        // Create indexes
        CreateIndexes();
    }

    public async Task<TProjection?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = CreateTenantFilter() & Builders<TProjection>.Filter.Eq(p => p.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projection {ProjectionType} with ID {Id}", typeof(TProjection).Name, id);
            throw;
        }
    }

    public async Task<IEnumerable<TProjection>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = CreateTenantFilter();
            var result = await _collection.Find(filter)
                .Skip(skip)
                .Limit(take)
                .ToListAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all projections {ProjectionType}", typeof(TProjection).Name);
            throw;
        }
    }

    public async Task<IEnumerable<TProjection>> FindAsync(Expression<Func<TProjection, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = CreateTenantFilter() & predicate;
            var result = await _collection.Find(filter).ToListAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding projections {ProjectionType}", typeof(TProjection).Name);
            throw;
        }
    }

    public async Task UpsertAsync(TProjection projection, CancellationToken cancellationToken = default)
    {
        try
        {
            // Set tenant ID if projection supports it
            if (projection is BaseProjection baseProjection && _tenantContext.HasTenant)
            {
                baseProjection.TenantId = _tenantContext.TenantId;
            }

            projection.LastUpdated = DateTime.UtcNow;

            var filter = CreateTenantFilter() & Builders<TProjection>.Filter.Eq(p => p.Id, projection.Id);
            await _collection.ReplaceOneAsync(filter, projection, new ReplaceOptions { IsUpsert = true }, cancellationToken);

            _logger.LogDebug("Upserted projection {ProjectionType} with ID {Id}", typeof(TProjection).Name, projection.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting projection {ProjectionType} with ID {Id}", typeof(TProjection).Name, projection.Id);
            throw;
        }
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = CreateTenantFilter() & Builders<TProjection>.Filter.Eq(p => p.Id, id);
            var result = await _collection.DeleteOneAsync(filter, cancellationToken);

            if (result.DeletedCount > 0)
            {
                _logger.LogDebug("Deleted projection {ProjectionType} with ID {Id}", typeof(TProjection).Name, id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting projection {ProjectionType} with ID {Id}", typeof(TProjection).Name, id);
            throw;
        }
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = CreateTenantFilter();
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting projections {ProjectionType}", typeof(TProjection).Name);
            throw;
        }
    }

    private FilterDefinition<TProjection> CreateTenantFilter()
    {
        if (!_tenantContext.HasTenant)
            return Builders<TProjection>.Filter.Empty;

        // Check if projection supports tenant isolation
        if (typeof(TProjection).IsAssignableTo(typeof(BaseProjection)))
        {
            return Builders<TProjection>.Filter.Or(
                Builders<TProjection>.Filter.Eq("TenantId", _tenantContext.TenantId),
                Builders<TProjection>.Filter.Eq("TenantId", BsonNull.Value),
                Builders<TProjection>.Filter.Exists("TenantId", false)
            );
        }

        return Builders<TProjection>.Filter.Empty;
    }

    private void CreateIndexes()
    {
        try
        {
            // Create index on Id field
            var idIndex = Builders<TProjection>.IndexKeys.Ascending(p => p.Id);
            _collection.Indexes.CreateOne(new CreateIndexModel<TProjection>(idIndex, new CreateIndexOptions { Unique = true }));

            // Create index on LastUpdated for time-based queries
            var lastUpdatedIndex = Builders<TProjection>.IndexKeys.Descending(p => p.LastUpdated);
            _collection.Indexes.CreateOne(new CreateIndexModel<TProjection>(lastUpdatedIndex));

            // Create tenant index if projection supports it
            if (typeof(TProjection).IsAssignableTo(typeof(BaseProjection)))
            {
                var tenantIndex = Builders<TProjection>.IndexKeys.Ascending("TenantId");
                _collection.Indexes.CreateOne(new CreateIndexModel<TProjection>(tenantIndex));

                // Compound index for tenant + LastUpdated
                var compoundIndex = Builders<TProjection>.IndexKeys
                    .Ascending("TenantId")
                    .Descending(p => p.LastUpdated);
                _collection.Indexes.CreateOne(new CreateIndexModel<TProjection>(compoundIndex));
            }

            _logger.LogDebug("Created indexes for projection {ProjectionType}", typeof(TProjection).Name);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create indexes for projection {ProjectionType}", typeof(TProjection).Name);
        }
    }
}