using System.Linq.Expressions;

namespace Marventa.Framework.Core.Domain.Specification;

/// <summary>
/// Specification pattern interface for building complex query criteria in a reusable and composable way.
/// Specifications encapsulate query logic and can be combined using AND, OR, and NOT operations.
/// </summary>
/// <typeparam name="T">The entity type this specification applies to.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Gets the filter criteria expression (WHERE clause).
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Gets the list of navigation properties to include (eager loading).
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Gets the list of navigation properties to include as strings (for complex includes).
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Gets the ascending order by expression.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Gets the descending order by expression.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Gets the number of records to take (for pagination or limiting results).
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Gets the number of records to skip (for pagination).
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Gets a value indicating whether paging is enabled.
    /// </summary>
    bool IsPagingEnabled { get; }
}
