using System.Diagnostics;
using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Infrastructure.Observability;

public class ActivityService : IActivityService
{
    private static readonly ActivitySource ActivitySource = new("Marventa.Framework");

    public IDisposable? StartActivity(string name)
    {
        var activity = ActivitySource.StartActivity(name);
        return activity;
    }

    public IDisposable? StartActivity(string name, Dictionary<string, object?> tags)
    {
        var activity = ActivitySource.StartActivity(name);
        if (activity != null)
        {
            foreach (var tag in tags)
            {
                activity.SetTag(tag.Key, tag.Value);
            }
        }
        return activity;
    }

    public void AddTag(string key, object? value)
    {
        Activity.Current?.SetTag(key, value);
    }

    public void AddEvent(string name)
    {
        Activity.Current?.AddEvent(new ActivityEvent(name, DateTimeOffset.UtcNow));
    }

    public void AddEvent(string name, Dictionary<string, object?> tags)
    {
        var tagCollection = new ActivityTagsCollection();
        foreach (var tag in tags)
        {
            tagCollection.Add(tag.Key, tag.Value);
        }
        Activity.Current?.AddEvent(new ActivityEvent(name, DateTimeOffset.UtcNow, tagCollection));
    }

    public void SetStatus(bool success, string? description = null)
    {
        if (Activity.Current != null)
        {
            var statusCode = success ? ActivityStatusCode.Ok : ActivityStatusCode.Error;
            Activity.Current.SetStatus(statusCode, description);
        }
    }

    public static ActivitySource GetActivitySource() => ActivitySource;
}