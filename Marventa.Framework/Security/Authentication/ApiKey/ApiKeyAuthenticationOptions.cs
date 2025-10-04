using Microsoft.AspNetCore.Authentication;

namespace Marventa.Framework.Security.Authentication.ApiKey;

/// <summary>
/// Options for API Key authentication.
/// </summary>
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// The default authentication scheme name for API Key authentication.
    /// </summary>
    public const string DefaultScheme = "ApiKey";

    /// <summary>
    /// Gets or sets the name of the header containing the API key.
    /// Default is "X-API-Key".
    /// </summary>
    public string HeaderName { get; set; } = "X-API-Key";

    /// <summary>
    /// Gets or sets a value indicating whether to require API key for all requests.
    /// Default is true.
    /// </summary>
    public bool RequireApiKey { get; set; } = true;
}
