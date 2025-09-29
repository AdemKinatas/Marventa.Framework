namespace Marventa.Framework.Core.Interfaces.Projections;

public abstract class BaseProjection : IProjection
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string Version { get; set; } = "1.0";
    public string? TenantId { get; set; }

    protected void UpdateVersion()
    {
        var parts = Version.Split('.');
        if (parts.Length >= 2 && int.TryParse(parts[1], out var minor))
        {
            Version = $"{parts[0]}.{minor + 1}";
        }
        LastUpdated = DateTime.UtcNow;
    }
}