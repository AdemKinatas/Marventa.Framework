namespace Marventa.Framework.Core.Domain;

/// <summary>
/// Interface for entities that support audit tracking.
/// Provides properties to track creation, modification, and soft deletion.
/// </summary>
public interface IAuditable : ISoftDeletable
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    Guid? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the entity.
    /// </summary>
    Guid? UpdatedBy { get; set; }
}
