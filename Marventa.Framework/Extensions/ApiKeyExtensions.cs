using Marventa.Framework.Security.Authentication.ApiKey;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Extension methods for configuring API Key authentication.
/// </summary>
public static class ApiKeyExtensions
{
    /// <summary>
    /// Adds API Key authentication to the service collection.
    /// Uses the default in-memory API key validator.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional action to configure authentication options.</param>
    /// <returns>The authentication builder for further configuration.</returns>
    public static AuthenticationBuilder AddApiKeyAuthentication(
        this IServiceCollection services,
        Action<ApiKeyAuthenticationOptions>? configureOptions = null)
    {
        // Register the default in-memory validator
        services.AddSingleton<IApiKeyValidator, InMemoryApiKeyValidator>();

        return services.AddApiKeyAuthentication<InMemoryApiKeyValidator>(configureOptions);
    }

    /// <summary>
    /// Adds API Key authentication to the service collection with a custom validator.
    /// </summary>
    /// <typeparam name="TValidator">The type of API key validator to use.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional action to configure authentication options.</param>
    /// <returns>The authentication builder for further configuration.</returns>
    public static AuthenticationBuilder AddApiKeyAuthentication<TValidator>(
        this IServiceCollection services,
        Action<ApiKeyAuthenticationOptions>? configureOptions = null)
        where TValidator : class, IApiKeyValidator
    {
        // Register the custom validator
        services.AddScoped<IApiKeyValidator, TValidator>();

        var builder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
        })
        .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            ApiKeyAuthenticationOptions.DefaultScheme,
            configureOptions);

        return builder;
    }

    /// <summary>
    /// Adds API Key authentication as an additional authentication scheme.
    /// This allows using API Key authentication alongside other schemes (e.g., JWT).
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="configureOptions">Optional action to configure authentication options.</param>
    /// <returns>The authentication builder for further configuration.</returns>
    public static AuthenticationBuilder AddApiKey(
        this AuthenticationBuilder builder,
        Action<ApiKeyAuthenticationOptions>? configureOptions = null)
    {
        builder.Services.AddSingleton<IApiKeyValidator, InMemoryApiKeyValidator>();

        return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            ApiKeyAuthenticationOptions.DefaultScheme,
            configureOptions);
    }
}
