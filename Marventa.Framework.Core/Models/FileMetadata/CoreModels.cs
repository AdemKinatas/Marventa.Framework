namespace Marventa.Framework.Core.Models.FileMetadata;

/// <summary>
/// Comprehensive file metadata model
/// </summary>
public class FileMetadata
{
    /// <summary>
    /// Unique file identifier
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Original filename
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// MIME type
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File extension
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// MD5 hash for integrity verification
    /// </summary>
    public string? MD5Hash { get; set; }

    /// <summary>
    /// SHA256 hash for security
    /// </summary>
    public string? SHA256Hash { get; set; }

    /// <summary>
    /// When the file was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the file was last modified
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// When metadata was last updated
    /// </summary>
    public DateTime MetadataUpdatedAt { get; set; }

    /// <summary>
    /// When the file metadata was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// File title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// File description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// File analytics data
    /// </summary>
    public FileAnalytics? Analytics { get; set; }

    /// <summary>
    /// User who uploaded the file
    /// </summary>
    public string? UploadedBy { get; set; }

    /// <summary>
    /// Tags associated with the file
    /// </summary>
    public List<FileTag> Tags { get; set; } = new();

    /// <summary>
    /// Custom metadata properties
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();

    /// <summary>
    /// File-specific metadata (image dimensions, video duration, etc.)
    /// </summary>
    public FileTypeMetadata? TypeSpecificMetadata { get; set; }
}

/// <summary>
/// File tag with metadata
/// </summary>
public class FileTag
{
    public string Tag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TagSource Source { get; set; }
    public double? Confidence { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Type-specific metadata for different file types
/// </summary>
public abstract class FileTypeMetadata
{
    public string FileType { get; set; } = string.Empty;
}

/// <summary>
/// Image-specific metadata
/// </summary>
public class ImageMetadata : FileTypeMetadata
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int ColorDepth { get; set; }
    public bool HasTransparency { get; set; }
    public string? ColorSpace { get; set; }
    public ExifData? Exif { get; set; }
    public List<string> DetectedObjects { get; set; } = new();
    public List<string> GeneratedTags { get; set; } = new();
}

/// <summary>
/// Video-specific metadata
/// </summary>
public class VideoMetadata : FileTypeMetadata
{
    public int Width { get; set; }
    public int Height { get; set; }
    public TimeSpan Duration { get; set; }
    public double FrameRate { get; set; }
    public long BitRate { get; set; }
    public string? VideoCodec { get; set; }
    public string? AudioCodec { get; set; }
    public List<string> AudioTracks { get; set; } = new();
    public List<string> Subtitles { get; set; } = new();
}

/// <summary>
/// Audio-specific metadata
/// </summary>
public class AudioMetadata : FileTypeMetadata
{
    public TimeSpan Duration { get; set; }
    public int BitRate { get; set; }
    public int SampleRate { get; set; }
    public int Channels { get; set; }
    public string? Codec { get; set; }
    public string? Artist { get; set; }
    public string? Album { get; set; }
    public string? Title { get; set; }
    public string? Genre { get; set; }
    public int? Year { get; set; }
    public int? TrackNumber { get; set; }
}

/// <summary>
/// File analytics data
/// </summary>
public class FileAnalytics
{
    public int TotalViews { get; set; }
    public int TotalDownloads { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public double AverageRating { get; set; }
    public int UniqueViewers { get; set; }
}

/// <summary>
/// Document-specific metadata
/// </summary>
public class DocumentMetadata : FileTypeMetadata
{
    public int PageCount { get; set; }
    public string? Author { get; set; }
    public string? Subject { get; set; }
    public string? Keywords { get; set; }
    public string? Creator { get; set; }
    public string? Producer { get; set; }
    public DateTime? CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public string? Language { get; set; }
    public bool IsEncrypted { get; set; }
    public bool HasDigitalSignature { get; set; }
}

/// <summary>
/// EXIF data for images
/// </summary>
public class ExifData
{
    public string? CameraMake { get; set; }
    public string? CameraModel { get; set; }
    public DateTime? DateTaken { get; set; }
    public string? Orientation { get; set; }
    public double? ExposureTime { get; set; }
    public double? FNumber { get; set; }
    public int? ISO { get; set; }
    public double? FocalLength { get; set; }
    public string? Flash { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Altitude { get; set; }
    public Dictionary<string, object> RawExif { get; set; } = new();
}