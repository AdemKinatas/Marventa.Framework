using System;
using System.Threading.Tasks;
using Marventa.Framework.Core.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Web.Security;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtAuthenticationMiddleware> _logger;

    public JwtAuthenticationMiddleware(RequestDelegate next, ILogger<JwtAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        var token = ExtractTokenFromRequest(context);

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var principal = await tokenService.ValidateTokenAsync(token);
                if (principal != null)
                {
                    context.User = principal;
                    _logger.LogDebug("JWT token validated successfully");
                }
                else
                {
                    _logger.LogWarning("Invalid JWT token provided");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JWT token");
            }
        }

        await _next(context);
    }

    private static string? ExtractTokenFromRequest(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader))
            return null;

        if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authorizationHeader["Bearer ".Length..];
        }

        return null;
    }
}