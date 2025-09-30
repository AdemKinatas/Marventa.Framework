using System;

namespace Marventa.Framework.Core.Entities;

/// <summary>
/// Base entity class providing common properties for all domain entities.
/// Includes automatic audit tracking, soft delete support, and unique identification.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for this entity.
    /// Automatically generated as a new GUID on entity creation.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this entity was created (UTC).
    /// Automatically set to current UTC time on entity creation.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this entity was last updated (UTC).
    /// Null if the entity has never been updated.
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created this entity.
    /// Null if creator information is not available.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated this entity.
    /// Null if updater information is not available or entity has never been updated.
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this entity has been soft deleted.
    /// Soft deleted entities are filtered out from queries by default using global query filters.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this entity was soft deleted (UTC).
    /// Null if the entity has not been deleted.
    /// </summary>
    public DateTime? DeletedDate { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who soft deleted this entity.
    /// Null if deletion information is not available or entity is not deleted.
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class.
    /// Automatically generates a new GUID for Id and sets CreatedDate to current UTC time.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
        IsDeleted = false;
    }
}