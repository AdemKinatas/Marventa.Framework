namespace Marventa.Framework.Core.Interfaces.Storage;

public class StorageFile
{
    public string Key { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ETag { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public string Url { get; set; } = string.Empty;
}