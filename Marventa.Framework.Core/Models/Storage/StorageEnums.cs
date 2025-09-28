namespace Marventa.Framework.Core.Models.Storage;

/// <summary>
/// Storage classes/tiers
/// </summary>
public enum StorageClass
{
    Standard,
    StandardIA,
    ReducedRedundancy,
    Archive,
    DeepArchive,
    Hot,
    Cool,
    Cold
}

/// <summary>
/// Public access levels
/// </summary>
public enum PublicAccessLevel
{
    Private,
    PublicRead,
    PublicReadWrite
}

/// <summary>
/// Storage permissions
/// </summary>
[Flags]
public enum StoragePermissions
{
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    ReadACL = 8,
    WriteACL = 16,
    FullControl = Read | Write | Delete | ReadACL | WriteACL
}

/// <summary>
/// Encryption methods
/// </summary>
public enum EncryptionMethod
{
    None,
    ServerSide,
    ClientSide,
    CustomerManaged
}

/// <summary>
/// Sort criteria for file listings
/// </summary>
public enum StorageSortBy
{
    Name,
    Size,
    CreatedDate,
    ModifiedDate,
    Extension
}

/// <summary>
/// Sort directions
/// </summary>
public enum SortDirection
{
    Ascending,
    Descending
}

/// <summary>
/// Storage grouping options for analytics
/// </summary>
public enum StorageGroupBy
{
    Hour,
    Day,
    Week,
    Month,
    Year
}

/// <summary>
/// Health status levels
/// </summary>
public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy,
    Unknown
}