namespace Marventa.Framework.Core.Models.FileMetadata;

/// <summary>
/// Represents a change in metadata
/// </summary>
public class MetadataChange
{
    /// <summary>
    /// Type of change
    /// </summary>
    public MetadataChangeType ChangeType { get; set; }

    /// <summary>
    /// Property that was changed
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Previous value
    /// </summary>
    public object? OldValue { get; set; }

    /// <summary>
    /// New value
    /// </summary>
    public object? NewValue { get; set; }

    /// <summary>
    /// When the change was made
    /// </summary>
    public DateTime ChangedAt { get; set; }
}

/// <summary>
/// File size range for filtering
/// </summary>
public class FileSizeRange
{
    /// <summary>
    /// Minimum file size in bytes (inclusive)
    /// </summary>
    public long? MinBytes { get; set; }

    /// <summary>
    /// Maximum file size in bytes (inclusive)
    /// </summary>
    public long? MaxBytes { get; set; }
}

/// <summary>
/// Types of metadata changes
/// </summary>
public enum MetadataChangeType
{
    Added,
    Modified,
    Removed,
    Renamed
}
