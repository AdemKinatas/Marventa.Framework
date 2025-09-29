namespace Marventa.Framework.Infrastructure.Services.Security;

public class JwtKeyRotationOptions
{
    public const string SectionName = "JwtKeyRotation";

    public TimeSpan RotationInterval { get; set; } = TimeSpan.FromDays(7);
    public TimeSpan ActiveKeyLifetime { get; set; } = TimeSpan.FromDays(1);
    public TimeSpan KeyValidityPeriod { get; set; } = TimeSpan.FromDays(30);
    public string Algorithm { get; set; } = "HS512";
    public bool EnableAutomaticRotation { get; set; } = true;
}