using System.Security.Claims;

namespace Marventa.Framework.Security.Authentication.ApiKey;

/// <summary>
/// Interface for validating API keys and creating claims principals.
/// </summary>
public interface IApiKeyValidator
{
    /// <summary>
    /// Validates an API key and returns a claims principal if valid.
    /// </summary>
    /// <param name="apiKey">The API key to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A ClaimsPrincipal if the API key is valid, otherwise null.</returns>
    Task<ClaimsPrincipal?> ValidateAsync(string apiKey, CancellationToken cancellationToken = default);
}
