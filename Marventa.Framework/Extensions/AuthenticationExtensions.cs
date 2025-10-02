using Marventa.Framework.Configuration;
using Marventa.Framework.Security.Authentication;
using Marventa.Framework.Security.Authentication.Abstractions;
using Marventa.Framework.Security.Authentication.Services;
using Marventa.Framework.Security.Authorization;
using Marventa.Framework.Security.Encryption;
using Marventa.Framework.Security.Encryption.Abstractions;
using Marventa.Framework.Security.Encryption.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Provides extension methods for configuring authentication and authorization services.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Adds JWT authentication and authorization services to the service collection.
    /// Configures JWT Bearer authentication with token validation parameters from configuration.
    /// Includes support for access tokens and refresh tokens.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when JWT configuration is missing or invalid.</exception>
    public static IServiceCollection AddMarventaAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.HasSection(ConfigurationKeys.Jwt))
        {
            return services;
        }

        // Register JWT configuration
        services.Configure<JwtConfiguration>(configuration.GetSection(ConfigurationKeys.Jwt));

        // Register security services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();

        // Configure JWT Bearer authentication
        var jwtSecret = configuration.GetRequiredValue(ConfigurationKeys.JwtSecret);
        var jwtIssuer = configuration.GetRequiredValue(ConfigurationKeys.JwtIssuer);
        var jwtAudience = configuration.GetRequiredValue(ConfigurationKeys.JwtAudience);

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        {
            KeyId = "Marventa-JWT-Key" // Set KeyId for kid header
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = signingKey
            };

            // Configure async event handlers for token validation
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers["Token-Expired"] = "true";
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        error = "unauthorized",
                        message = "You are not authorized to access this resource."
                    });
                    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(result));
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Adds authorization services with permission-based policy provider.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        return services;
    }

    /// <summary>
    /// Adds both authentication and authorization services in a single call.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaAuthenticationAndAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMarventaAuthentication(configuration);
        services.AddMarventaAuthorization();

        return services;
    }
}
