using Marventa.Framework.Core.Entities;
using Marventa.Framework.Core.Events;
using Marventa.Framework.Core.Interfaces.MultiTenancy;
using Marventa.Framework.Core.Interfaces.Events;
using Microsoft.EntityFrameworkCore;

namespace Marventa.Framework.Infrastructure.Data;

/// <summary>
/// Base DbContext providing common functionality for all database contexts
/// </summary>
public abstract class BaseDbContext : DbContext
{
    private readonly ITenantContext? _tenantContext;

    protected BaseDbContext(DbContextOptions options) : base(options)
    {
    }

    protected BaseDbContext(DbContextOptions options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Apply audit information
        ApplyAuditInformation();

        // Apply soft delete
        ApplySoftDelete();

        // Apply multi-tenancy
        ApplyMultiTenancy();

        // Dispatch domain events before saving
        await DispatchDomainEventsAsync(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        // Apply audit information
        ApplyAuditInformation();

        // Apply soft delete
        ApplySoftDelete();

        // Apply multi-tenancy
        ApplyMultiTenancy();

        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filters for soft delete
        ApplyGlobalQueryFilters(modelBuilder);

        // Apply multi-tenancy filters
        ApplyMultiTenancyFilters(modelBuilder);
    }

    private void ApplyAuditInformation()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    // Soft delete handling is done in ApplySoftDelete
                    break;
            }
        }
    }

    private void ApplySoftDelete()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedDate = DateTime.UtcNow;
        }
    }

    private void ApplyMultiTenancy()
    {
        if (_tenantContext?.HasTenant != true)
            return;

        var entries = ChangeTracker.Entries<ITenantEntity>()
            .Where(e => e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.Entity.TenantId))
            {
                entry.Entity.TenantId = _tenantContext.TenantId;
            }
        }
    }

    private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var filter = System.Linq.Expressions.Expression.Lambda(
                    System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false)),
                    parameter
                );

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    private void ApplyMultiTenancyFilters(ModelBuilder modelBuilder)
    {
        if (_tenantContext?.HasTenant != true)
            return;

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ITenantEntity.TenantId));
                var tenantId = System.Linq.Expressions.Expression.Constant(_tenantContext.TenantId);
                var filter = System.Linq.Expressions.Expression.Lambda(
                    System.Linq.Expressions.Expression.Equal(property, tenantId),
                    parameter
                );

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        // Domain events functionality is not yet implemented
        // Uncomment when BaseAggregateRoot with DomainEvents support is added to Core project
        /*
        var domainEntities = ChangeTracker
            .Entries<BaseAggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            // Domain events should be published via a domain event dispatcher
            // This is a placeholder - implement based on your event handling strategy
            await PublishDomainEventAsync(domainEvent, cancellationToken);
        }
        */
        await Task.CompletedTask;
    }

    /// <summary>
    /// Override this method to implement custom domain event publishing logic
    /// </summary>
    protected virtual Task PublishDomainEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // Default implementation does nothing
        // Override in derived context to implement actual publishing
        return Task.CompletedTask;
    }
}