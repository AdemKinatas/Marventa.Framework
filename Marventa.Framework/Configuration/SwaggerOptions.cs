namespace Marventa.Framework.Configuration;

public class SwaggerOptions
{
    public const string SectionName = "Swagger";

    public bool Enabled { get; set; } = true;
    public string Title { get; set; } = "API";
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = "v1";
    public bool RequireAuthorization { get; set; } = true;
    public string[] EnvironmentRestriction { get; set; } = new[] { "Development" };
    public ContactInfo? Contact { get; set; }
    public LicenseInfo? License { get; set; }

    public class ContactInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class LicenseInfo
    {
        public string Name { get; set; } = "MIT";
        public string Url { get; set; } = string.Empty;
    }
}
