namespace Marventa.Framework.Web.HealthChecks;

public class HealthCheckEntry
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TimeSpan Duration { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}