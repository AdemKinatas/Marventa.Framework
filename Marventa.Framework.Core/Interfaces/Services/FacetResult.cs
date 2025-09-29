namespace Marventa.Framework.Core.Interfaces.Services;

public class FacetResult
{
    public string Field { get; set; } = string.Empty;
    public List<FacetValue> Values { get; set; } = new();
}