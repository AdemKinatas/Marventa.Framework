namespace Marventa.Framework.Web.Versioning;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ApiVersionAttribute : Attribute
{
    public string Version { get; }
    public bool IsDeprecated { get; set; }
    public string? DeprecatedMessage { get; set; }

    public ApiVersionAttribute(string version)
    {
        Version = version ?? throw new ArgumentNullException(nameof(version));
    }

    public ApiVersionAttribute(string version, bool isDeprecated, string? deprecatedMessage = null)
    {
        Version = version ?? throw new ArgumentNullException(nameof(version));
        IsDeprecated = isDeprecated;
        DeprecatedMessage = deprecatedMessage;
    }
}