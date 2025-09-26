namespace Marventa.Framework.Web.HealthChecks;

public class HealthReport
{
    public string Status { get; set; } = string.Empty;
    public TimeSpan TotalDuration { get; set; }
    public List<HealthCheckEntry> Entries { get; set; } = new();
}