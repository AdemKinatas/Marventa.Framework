using System.Linq.Expressions;

namespace Marventa.Framework.Core.Domain.Specification;

/// <summary>
/// Base implementation of the Specification pattern.
/// Provides a fluent API for building complex query specifications.
/// </summary>
/// <typeparam name="T">The entity type this specification applies to.</typeparam>
public abstract class BaseSpecification<T> : ISpecification<T>
{
    /// <inheritdoc/>
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <inheritdoc/>
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    /// <inheritdoc/>
    public List<string> IncludeStrings { get; } = new();

    /// <inheritdoc/>
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <inheritdoc/>
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <inheritdoc/>
    public int? Take { get; private set; }

    /// <inheritdoc/>
    public int? Skip { get; private set; }

    /// <inheritdoc/>
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSpecification{T}"/> class.
    /// </summary>
    protected BaseSpecification()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSpecification{T}"/> class with a criteria.
    /// </summary>
    /// <param name="criteria">The filter criteria.</param>
    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Adds a navigation property to include in the query (eager loading).
    /// </summary>
    /// <param name="includeExpression">The include expression.</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        if (includeExpression == null)
            throw new ArgumentNullException(nameof(includeExpression));

        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Adds a navigation property to include in the query using a string path (for complex includes).
    /// </summary>
    /// <param name="includeString">The include path as string (e.g., "Orders.OrderItems").</param>
    protected void AddInclude(string includeString)
    {
        if (string.IsNullOrWhiteSpace(includeString))
            throw new ArgumentException("Include string cannot be null or empty.", nameof(includeString));

        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Applies paging to the query.
    /// </summary>
    /// <param name="skip">The number of records to skip.</param>
    /// <param name="take">The number of records to take.</param>
    protected void ApplyPaging(int skip, int take)
    {
        if (skip < 0)
            throw new ArgumentException("Skip must be greater than or equal to 0.", nameof(skip));

        if (take <= 0)
            throw new ArgumentException("Take must be greater than 0.", nameof(take));

        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// Applies ascending ordering to the query.
    /// </summary>
    /// <param name="orderByExpression">The order by expression.</param>
    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        if (orderByExpression == null)
            throw new ArgumentNullException(nameof(orderByExpression));

        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Applies descending ordering to the query.
    /// </summary>
    /// <param name="orderByDescendingExpression">The order by descending expression.</param>
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        if (orderByDescendingExpression == null)
            throw new ArgumentNullException(nameof(orderByDescendingExpression));

        OrderByDescending = orderByDescendingExpression;
    }
}
