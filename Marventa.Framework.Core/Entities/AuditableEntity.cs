using System;

namespace Marventa.Framework.Core.Entities;

/// <summary>
/// Auditable entity class extending <see cref="BaseEntity"/> with versioning capabilities.
/// Provides optimistic concurrency control through RowVersion and semantic versioning.
/// Use this for entities that require version tracking and concurrency conflict detection.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the semantic version of this entity (e.g., "1.0", "2.1").
    /// Can be used to track major changes or revisions to the entity.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the row version for optimistic concurrency control.
    /// This is typically a timestamp or incrementing value used by Entity Framework
    /// to detect concurrent modifications. Automatically managed by the database.
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}