namespace Marventa.Framework.Core.Interfaces.Services;

public class SearchResult<T>
{
    public List<T> Items { get; set; } = new();
    public long TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public double SearchTime { get; set; }
    public Dictionary<string, List<string>>? Highlights { get; set; }
    public Dictionary<string, FacetResult>? Facets { get; set; }
}