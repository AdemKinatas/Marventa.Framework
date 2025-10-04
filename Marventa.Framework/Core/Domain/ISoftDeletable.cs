namespace Marventa.Framework.Core.Domain;

/// <summary>
/// Interface for entities that support soft delete functionality.
/// Entities implementing this interface will not be physically deleted from the database,
/// but marked as deleted instead.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is deleted.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was deleted.
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// </summary>
    Guid? DeletedBy { get; set; }
}
