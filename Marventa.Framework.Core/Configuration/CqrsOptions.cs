using System.Reflection;

namespace Marventa.Framework.Core.Configuration;

/// <summary>
/// CQRS and MediatR configuration options
/// </summary>
public class CqrsOptions
{
    /// <summary>
    /// Assemblies to scan for MediatR handlers, commands, queries, and validators
    /// </summary>
    public List<Assembly> Assemblies { get; set; } = new();

    /// <summary>
    /// Enable automatic transaction management for commands
    /// </summary>
    public bool EnableTransactionBehavior { get; set; } = true;

    /// <summary>
    /// Enable automatic logging and performance monitoring
    /// </summary>
    public bool EnableLoggingBehavior { get; set; } = true;

    /// <summary>
    /// Enable automatic request validation using FluentValidation
    /// </summary>
    public bool EnableValidationBehavior { get; set; } = true;
}