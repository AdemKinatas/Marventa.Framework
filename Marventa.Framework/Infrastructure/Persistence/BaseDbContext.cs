using Marventa.Framework.Core.Domain;
using Marventa.Framework.Infrastructure.Persistence.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;

namespace Marventa.Framework.Infrastructure;

public abstract class BaseDbContext : DbContext
{
    private readonly IMediator? _mediator;
    private readonly bool _useOutbox;

    /// <summary>
    /// Gets or sets the DbSet for Outbox messages.
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    protected BaseDbContext(DbContextOptions options) : base(options)
    {
        _useOutbox = false;
    }

    protected BaseDbContext(DbContextOptions options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
        _useOutbox = false;
    }

    protected BaseDbContext(DbContextOptions options, IMediator mediator, bool useOutbox) : base(options)
    {
        _mediator = mediator;
        _useOutbox = useOutbox;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();

        // Collect domain events BEFORE saving to avoid detached entity issues
        var eventsToDispatch = CollectDomainEvents();

        // If outbox pattern is enabled, save events to outbox instead of dispatching
        if (_useOutbox && eventsToDispatch.Any())
        {
            await SaveEventsToOutboxAsync(eventsToDispatch, cancellationToken);
        }

        // Save changes to database first
        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch events AFTER successful save (only if not using outbox)
        if (!_useOutbox)
        {
            await DispatchDomainEventsAsync(eventsToDispatch, cancellationToken);
        }

        return result;
    }

    /// <summary>
    /// Updates auditable entities with timestamps and handles soft delete.
    /// Works with any entity type that implements auditable properties.
    /// </summary>
    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity.GetType().IsSubclassOf(typeof(AuditableEntity<>).MakeGenericType(
                e.Entity.GetType().BaseType?.GetGenericArguments().FirstOrDefault() ?? typeof(Guid))));

        foreach (var entry in entries)
        {
            if (entry.Entity is not IAuditable auditable)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    auditable.CreatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    auditable.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    if (entry.Entity is ISoftDeletable softDeletable)
                    {
                        entry.State = EntityState.Modified;
                        softDeletable.IsDeleted = true;
                        softDeletable.DeletedAt = DateTime.UtcNow;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Collects domain events from tracked entities before saving.
    /// </summary>
    /// <returns>List of domain events to dispatch after save.</returns>
    private List<IDomainEvent> CollectDomainEvents()
    {
        if (_mediator == null)
            return new List<IDomainEvent>();

        var domainEntities = ChangeTracker.Entries()
            .Where(e => e.Entity is IHasDomainEvents)
            .Select(e => (IHasDomainEvents)e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Clear events from entities after collection
        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        return domainEvents;
    }

    /// <summary>
    /// Dispatches collected domain events after successful save.
    /// </summary>
    /// <param name="domainEvents">Events to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task DispatchDomainEventsAsync(List<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        if (_mediator == null || !domainEvents.Any())
            return;

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }

    /// <summary>
    /// Saves domain events to the outbox table for later processing.
    /// This ensures events are persisted in the same transaction as business data.
    /// </summary>
    /// <param name="domainEvents">Events to save to outbox.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task SaveEventsToOutboxAsync(List<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents)
        {
            var eventType = domainEvent.GetType();
            var payload = JsonSerializer.Serialize(domainEvent, eventType);

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventType = $"{eventType.FullName}, {eventType.Assembly.GetName().Name}",
                Payload = payload,
                OccurredOn = DateTime.UtcNow,
                RetryCount = 0,
                MaxRetries = 3
            };

            await OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
