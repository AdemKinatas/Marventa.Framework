using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Marventa.Framework.Security.Authentication.ApiKey;

/// <summary>
/// Authentication handler for API Key authentication.
/// Validates the API key from the request header and creates a claims principal.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeyValidator _apiKeyValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthenticationHandler"/> class.
    /// </summary>
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyValidator apiKeyValidator)
        : base(options, logger, encoder)
    {
        _apiKeyValidator = apiKeyValidator ?? throw new ArgumentNullException(nameof(apiKeyValidator));
    }

    /// <inheritdoc/>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if the API key header is present
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var apiKeyHeaderValues))
        {
            return Options.RequireApiKey
                ? AuthenticateResult.Fail($"Missing {Options.HeaderName} header")
                : AuthenticateResult.NoResult();
        }

        var apiKey = apiKeyHeaderValues.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return AuthenticateResult.Fail($"Invalid {Options.HeaderName} header value");
        }

        // Validate the API key
        var principal = await _apiKeyValidator.ValidateAsync(apiKey, Context.RequestAborted);

        if (principal == null)
        {
            Logger.LogWarning("Invalid API key attempt: {ApiKey}", MaskApiKey(apiKey));
            return AuthenticateResult.Fail("Invalid API key");
        }

        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        Logger.LogInformation("API key authentication successful for: {Owner}",
            principal.Identity?.Name ?? "Unknown");

        return AuthenticateResult.Success(ticket);
    }

    /// <inheritdoc/>
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 401;
        Response.Headers["WWW-Authenticate"] = $"ApiKey realm=\"{Options.HeaderName}\"";
        return Task.CompletedTask;
    }

    /// <summary>
    /// Masks the API key for logging purposes (shows first 4 characters only).
    /// </summary>
    private static string MaskApiKey(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey.Length <= 4)
            return "****";

        return $"{apiKey.Substring(0, 4)}****";
    }
}
