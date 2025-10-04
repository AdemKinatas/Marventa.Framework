namespace Marventa.Framework.Core.Domain;

/// <summary>
/// Interface for entities that can raise domain events.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Gets the collection of domain events raised by this entity.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Adds a domain event to the entity.
    /// </summary>
    /// <param name="eventItem">The domain event to add.</param>
    void AddDomainEvent(IDomainEvent eventItem);

    /// <summary>
    /// Removes a domain event from the entity.
    /// </summary>
    /// <param name="eventItem">The domain event to remove.</param>
    void RemoveDomainEvent(IDomainEvent eventItem);

    /// <summary>
    /// Clears all domain events from the entity.
    /// </summary>
    void ClearDomainEvents();
}
