using Microsoft.EntityFrameworkCore;

namespace Marventa.Framework.Core.Domain.Specification;

/// <summary>
/// Evaluates specifications and applies them to IQueryable.
/// This class is responsible for translating specification objects into EF Core queries.
/// </summary>
public static class SpecificationEvaluator
{
    /// <summary>
    /// Applies a specification to a queryable and returns the modified query.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="inputQuery">The input queryable.</param>
    /// <param name="specification">The specification to apply.</param>
    /// <returns>The modified queryable with the specification applied.</returns>
    public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> specification) where T : class
    {
        if (inputQuery == null)
            throw new ArgumentNullException(nameof(inputQuery));

        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        var query = inputQuery;

        // Apply filter criteria (WHERE clause)
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Apply includes (eager loading)
        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        // Apply string-based includes (for complex navigation properties)
        query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        // Apply ordering
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Apply paging (must be after ordering)
        if (specification.IsPagingEnabled)
        {
            if (specification.Skip.HasValue)
            {
                query = query.Skip(specification.Skip.Value);
            }

            if (specification.Take.HasValue)
            {
                query = query.Take(specification.Take.Value);
            }
        }

        return query;
    }
}
