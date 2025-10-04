namespace Marventa.Framework.Core.Shared;

public static class LogMessages
{
    // Performance Middleware
    public const string SlowRequest = "Slow request: {Method} {Path} took {ElapsedMs}ms";
    public const string ResponseTimeAdditionFailed = "Could not add response time to JSON response";

    // Exception Middleware
    public const string UnhandledExceptionOccurred = "An unhandled exception occurred";

    // Content Types
    public const string ApplicationJson = "application/json";
}
