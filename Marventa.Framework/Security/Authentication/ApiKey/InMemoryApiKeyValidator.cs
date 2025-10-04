using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Marventa.Framework.Security.Authentication.ApiKey;

/// <summary>
/// In-memory implementation of API key validator.
/// Validates API keys against a configured list in appsettings.json.
/// For production use, consider implementing a database-backed validator.
/// </summary>
public class InMemoryApiKeyValidator : IApiKeyValidator
{
    private readonly Dictionary<string, ApiKeyInfo> _apiKeys;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryApiKeyValidator"/> class.
    /// </summary>
    /// <param name="configuration">The configuration containing API keys.</param>
    public InMemoryApiKeyValidator(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        _apiKeys = new Dictionary<string, ApiKeyInfo>();

        // Load API keys from configuration
        var apiKeysSection = configuration.GetSection("Authentication:ApiKeys");
        foreach (var section in apiKeysSection.GetChildren())
        {
            var key = section.GetValue<string>("Key");
            var owner = section.GetValue<string>("Owner");
            var roles = section.GetValue<string>("Roles")?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(owner))
            {
                _apiKeys[key] = new ApiKeyInfo
                {
                    Owner = owner,
                    Roles = roles
                };
            }
        }
    }

    /// <inheritdoc/>
    public Task<ClaimsPrincipal?> ValidateAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return Task.FromResult<ClaimsPrincipal?>(null);

        if (!_apiKeys.TryGetValue(apiKey, out var apiKeyInfo))
            return Task.FromResult<ClaimsPrincipal?>(null);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, apiKeyInfo.Owner),
            new Claim("ApiKey", apiKey)
        };

        // Add role claims
        foreach (var role in apiKeyInfo.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
        }

        var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationOptions.DefaultScheme);
        var principal = new ClaimsPrincipal(identity);

        return Task.FromResult<ClaimsPrincipal?>(principal);
    }

    /// <summary>
    /// Information about an API key.
    /// </summary>
    private class ApiKeyInfo
    {
        public string Owner { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
