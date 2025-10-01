namespace Marventa.Framework.Configuration;

public class ApiVersioningOptions
{
    public const string SectionName = "ApiVersioning";

    public bool Enabled { get; set; } = true;
    public string DefaultVersion { get; set; } = "1.0";
    public bool ReportApiVersions { get; set; } = true;
    public bool AssumeDefaultVersionWhenUnspecified { get; set; } = true;
    public VersioningType VersioningType { get; set; } = VersioningType.UrlSegment;
    public string HeaderName { get; set; } = "X-API-Version";
    public string QueryStringParameterName { get; set; } = "api-version";
}

public enum VersioningType
{
    UrlSegment,
    QueryString,
    Header,
    MediaType
}
