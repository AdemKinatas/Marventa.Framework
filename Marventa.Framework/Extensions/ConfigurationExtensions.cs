using Microsoft.Extensions.Configuration;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Provides extension methods for configuration validation and retrieval.
/// These methods help consuming applications safely read and validate configuration values.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Checks if a configuration section exists.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The section name to check.</param>
    /// <returns>True if the section exists; otherwise, false.</returns>
    public static bool HasSection(this IConfiguration configuration, string sectionName)
    {
        return configuration.GetSection(sectionName).Exists();
    }

    /// <summary>
    /// Gets a configuration value with a fallback default value.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="key">The configuration key.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The configuration value or the default value.</returns>
    public static string GetValueOrDefault(this IConfiguration configuration, string key, string defaultValue)
    {
        return configuration[key] ?? defaultValue;
    }

    /// <summary>
    /// Gets a required configuration value or throws an exception if not found.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration value is not found or empty.</exception>
    public static string GetRequiredValue(this IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Configuration value for '{key}' is required but was not found or is empty.");
        }
        return value;
    }

    /// <summary>
    /// Checks if a section is enabled based on the "Enabled" key.
    /// If the "Enabled" key is not present, the section is assumed to be enabled.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The section name to check.</param>
    /// <returns>True if the section is enabled; otherwise, false.</returns>
    public static bool IsSectionEnabled(this IConfiguration configuration, string sectionName)
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            return false;
        }

        var enabledValue = section["Enabled"];
        if (string.IsNullOrEmpty(enabledValue))
        {
            return true; // If no "Enabled" key exists, assume enabled
        }

        return bool.TryParse(enabledValue, out var isEnabled) && isEnabled;
    }

    /// <summary>
    /// Gets a strongly-typed configuration section with automatic instantiation if not found.
    /// </summary>
    /// <typeparam name="T">The type to bind the configuration section to.</typeparam>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The section name.</param>
    /// <returns>The bound configuration object or a new instance if not found.</returns>
    public static T GetConfigurationSection<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            return new T();
        }

        return section.Get<T>() ?? new T();
    }

    /// <summary>
    /// Gets a required strongly-typed configuration section or throws an exception.
    /// </summary>
    /// <typeparam name="T">The type to bind the configuration section to.</typeparam>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The section name.</param>
    /// <returns>The bound configuration object.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the section is not found or cannot be bound.</exception>
    public static T GetRequiredConfigurationSection<T>(this IConfiguration configuration, string sectionName) where T : class
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{sectionName}' is required but was not found.");
        }

        var result = section.Get<T>();
        if (result == null)
        {
            throw new InvalidOperationException($"Configuration section '{sectionName}' could not be bound to type {typeof(T).Name}.");
        }

        return result;
    }
}
