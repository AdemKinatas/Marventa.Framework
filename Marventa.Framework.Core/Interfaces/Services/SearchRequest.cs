namespace Marventa.Framework.Core.Interfaces.Services;

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Dictionary<string, object> Filters { get; set; } = new();
    public List<SortField> SortFields { get; set; } = new();
    public List<string> Fields { get; set; } = new();
    public bool IncludeHighlights { get; set; }
    public string? IndexName { get; set; }
}