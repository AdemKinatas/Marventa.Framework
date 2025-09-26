namespace Marventa.Framework.Core.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IdempotentAttribute : Attribute
{
    public string? KeyTemplate { get; set; }
    public int ExpirationHours { get; set; } = 24;
    public string? HeaderName { get; set; }
    public bool RequireKey { get; set; } = true;

    public IdempotentAttribute() { }

    public IdempotentAttribute(string keyTemplate)
    {
        KeyTemplate = keyTemplate;
    }

    public IdempotentAttribute(int expirationHours)
    {
        ExpirationHours = expirationHours;
    }

    public IdempotentAttribute(string keyTemplate, int expirationHours)
    {
        KeyTemplate = keyTemplate;
        ExpirationHours = expirationHours;
    }
}