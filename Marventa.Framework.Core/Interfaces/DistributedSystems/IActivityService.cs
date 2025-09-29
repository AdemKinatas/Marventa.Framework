namespace Marventa.Framework.Core.Interfaces.DistributedSystems;

public interface IActivityService
{
    IDisposable? StartActivity(string name);
    IDisposable? StartActivity(string name, Dictionary<string, object?> tags);
    void AddTag(string key, object? value);
    void AddEvent(string name);
    void AddEvent(string name, Dictionary<string, object?> tags);
    void SetStatus(bool success, string? description = null);
}