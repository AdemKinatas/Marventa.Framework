namespace Marventa.Framework.Core.Interfaces.Services;

public class SortField
{
    public string Field { get; set; } = string.Empty;
    public SortDirection Direction { get; set; } = SortDirection.Ascending;
}