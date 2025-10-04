using Marventa.Framework.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Extension methods for configuring Outbox pattern support.
/// </summary>
public static class OutboxExtensions
{
    /// <summary>
    /// Adds Outbox pattern support to the service collection.
    /// Registers the outbox repository and background processor.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOutbox(this IServiceCollection services)
    {
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddHostedService<OutboxProcessor>();

        return services;
    }

    /// <summary>
    /// Configures the OutboxMessage entity in the model builder.
    /// Call this method in your DbContext's OnModelCreating method.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    public static void ConfigureOutbox(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.EventType)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Payload)
                .IsRequired();

            entity.Property(e => e.OccurredOn)
                .IsRequired();

            entity.Property(e => e.ProcessedOn)
                .IsRequired(false);

            entity.Property(e => e.Error)
                .HasMaxLength(2000);

            entity.Property(e => e.RetryCount)
                .IsRequired();

            entity.Property(e => e.MaxRetries)
                .IsRequired();

            // Index for efficient querying of unprocessed messages
            entity.HasIndex(e => new { e.ProcessedOn, e.OccurredOn })
                .HasDatabaseName("IX_OutboxMessages_ProcessedOn_OccurredOn");
        });
    }
}
