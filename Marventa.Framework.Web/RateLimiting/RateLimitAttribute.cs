using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Marventa.Framework.Web.RateLimiting;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RateLimitAttribute : Attribute, IFilterMetadata
{
    public int MaxRequests { get; set; } = 10;
    public int WindowSizeMinutes { get; set; } = 1;
    public string? Policy { get; set; }

    public RateLimitAttribute() { }

    public RateLimitAttribute(int maxRequests, int windowSizeMinutes)
    {
        MaxRequests = maxRequests;
        WindowSizeMinutes = windowSizeMinutes;
    }

    public RateLimitAttribute(string policy)
    {
        Policy = policy;
    }
}