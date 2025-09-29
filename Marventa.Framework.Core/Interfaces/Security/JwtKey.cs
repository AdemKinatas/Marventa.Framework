namespace Marventa.Framework.Core.Interfaces.Security;

public class JwtKey
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Key { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string Algorithm { get; set; } = "HS256";
}