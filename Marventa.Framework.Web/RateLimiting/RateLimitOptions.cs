using System;

namespace Marventa.Framework.Web.RateLimiting;

public class RateLimitOptions
{
    public bool EnableRateLimiting { get; set; } = true;
    public int MaxRequests { get; set; } = 100;
    public TimeSpan WindowSize { get; set; } = TimeSpan.FromMinutes(1);
}