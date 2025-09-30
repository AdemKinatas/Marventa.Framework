using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Logging;

public static class StructuredLogging
{
    public static void LogStructured(
        this ILogger logger,
        LogLevel logLevel,
        string messageTemplate,
        params object[] propertyValues)
    {
        logger.Log(logLevel, messageTemplate, propertyValues);
    }

    public static void LogInformationStructured(
        this ILogger logger,
        string messageTemplate,
        params object[] propertyValues)
    {
        logger.LogInformation(messageTemplate, propertyValues);
    }

    public static void LogWarningStructured(
        this ILogger logger,
        string messageTemplate,
        params object[] propertyValues)
    {
        logger.LogWarning(messageTemplate, propertyValues);
    }

    public static void LogErrorStructured(
        this ILogger logger,
        Exception exception,
        string messageTemplate,
        params object[] propertyValues)
    {
        logger.LogError(exception, messageTemplate, propertyValues);
    }
}
