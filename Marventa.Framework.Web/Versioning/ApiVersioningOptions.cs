namespace Marventa.Framework.Web.Versioning;

public class ApiVersioningOptions
{
    public string DefaultVersion { get; set; } = "1.0";
    public bool AssumeDefaultVersionWhenUnspecified { get; set; } = true;
    public ApiVersionReader ApiVersionReader { get; set; } = ApiVersionReader.Header;
    public string HeaderName { get; set; } = "X-Api-Version";
    public string QueryParameterName { get; set; } = "version";
    public bool EnableApiExplorer { get; set; } = true;
    public string[] SupportedVersions { get; set; } = { "1.0" };
}

public enum ApiVersionReader
{
    Header,
    QueryParameter,
    UrlSegment,
    MediaType
}