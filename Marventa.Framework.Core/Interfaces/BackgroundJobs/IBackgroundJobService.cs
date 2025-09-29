using System.Linq.Expressions;

namespace Marventa.Framework.Core.Interfaces.BackgroundJobs;

public interface IBackgroundJobService
{
    string Enqueue<T>(Expression<Action<T>> methodCall);
    string Enqueue<T>(string queue, Expression<Action<T>> methodCall);
    string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);
    string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt);
    void RecurringJob<T>(string jobId, Expression<Action<T>> methodCall, string cronExpression);
    void RecurringJob<T>(string jobId, Expression<Action<T>> methodCall, string cronExpression, string? timeZone = null);
    bool Delete(string jobId);
    void DeleteRecurring(string recurringJobId);
}