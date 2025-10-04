namespace Marventa.Framework.Configuration;

/// <summary>
/// Configuration options for request/response logging.
/// </summary>
public class LoggingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether request/response logging is enabled.
    /// Default is false to avoid performance impact in production.
    /// </summary>
    public bool EnableRequestResponseLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum size of request/response body to log in bytes.
    /// Bodies larger than this will be truncated. Default is 4096 bytes (4KB).
    /// </summary>
    public int MaxBodyLogSize { get; set; } = 4096;

    /// <summary>
    /// Gets or sets the list of sensitive headers to mask in logs.
    /// </summary>
    public List<string> SensitiveHeaders { get; set; } = new()
    {
        "Authorization",
        "X-API-Key",
        "Cookie",
        "Set-Cookie"
    };

    /// <summary>
    /// Gets or sets the list of sensitive body field names to mask in logs.
    /// Field names are case-insensitive.
    /// </summary>
    public List<string> SensitiveBodyFields { get; set; } = new()
    {
        "password",
        "token",
        "secret",
        "apikey",
        "api_key",
        "accesstoken",
        "access_token",
        "refreshtoken",
        "refresh_token",
        "clientsecret",
        "client_secret"
    };

    /// <summary>
    /// Gets or sets a value indicating whether to log request headers.
    /// Default is true.
    /// </summary>
    public bool LogRequestHeaders { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to log request body.
    /// Default is true.
    /// </summary>
    public bool LogRequestBody { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to log response headers.
    /// Default is true.
    /// </summary>
    public bool LogResponseHeaders { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to log response body.
    /// Default is true.
    /// </summary>
    public bool LogResponseBody { get; set; } = true;
}
