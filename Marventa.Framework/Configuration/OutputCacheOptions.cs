namespace Marventa.Framework.Configuration;

/// <summary>
/// Configuration options for ASP.NET Core output caching.
/// </summary>
public class OutputCacheOptions
{
    public const string SectionName = "OutputCache";

    /// <summary>
    /// Gets or sets whether output caching is enabled.
    /// Default is false.
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the default cache duration in seconds.
    /// Default is 60 seconds.
    /// </summary>
    public int DefaultExpirationSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets whether to vary by query parameters.
    /// Default is true.
    /// </summary>
    public bool VaryByQuery { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to vary by header values.
    /// Default is false.
    /// </summary>
    public bool VaryByHeader { get; set; } = false;

    /// <summary>
    /// Gets or sets the list of header names to vary by when VaryByHeader is true.
    /// </summary>
    public string[] VaryByHeaderNames { get; set; } = Array.Empty<string>();
}
