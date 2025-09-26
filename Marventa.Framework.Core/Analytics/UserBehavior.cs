namespace Marventa.Framework.Core.Analytics;

public class UserBehavior
{
    public string UserId { get; set; } = string.Empty;
    public int PageViews { get; set; }
    public int Events { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public List<string> TopPages { get; set; } = new();
    public List<string> TopEvents { get; set; } = new();
    public Dictionary<string, int> EventCounts { get; set; } = new();
}