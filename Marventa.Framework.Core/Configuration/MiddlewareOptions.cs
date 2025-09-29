namespace Marventa.Framework.Core.Configuration;

/// <summary>
/// Configuration options for middleware pipeline
/// </summary>
public class MiddlewareOptions
{
    /// <summary>
    /// Use unified middleware for better performance (recommended)
    /// </summary>
    public bool UseUnifiedMiddleware { get; set; } = true;

    /// <summary>
    /// Use modular middleware for advanced customization
    /// </summary>
    public bool UseModularMiddleware { get; set; } = false;

    /// <summary>
    /// Rate limiting configuration
    /// </summary>
    public RateLimitConfig RateLimiting { get; set; } = new();
}