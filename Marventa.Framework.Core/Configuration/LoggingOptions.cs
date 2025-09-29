namespace Marventa.Framework.Core.Configuration;

/// <summary>
/// Configuration options for logging services
/// </summary>
public class LoggingOptions
{
    /// <summary>
    /// Logging provider name
    /// </summary>
    public string Provider { get; set; } = "Serilog";

    /// <summary>
    /// Minimum log level
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";
}