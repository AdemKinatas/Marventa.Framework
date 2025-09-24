using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Marventa.Framework.Infrastructure.Data;

public static class QueryOptimizer
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public static IQueryable<T> ApplyOrdering<T, TKey>(this IQueryable<T> query,
        Expression<Func<T, TKey>> keySelector, bool descending = false)
    {
        return descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }

    public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query,
        Expression<Func<T, bool>> predicate)
    {
        return query.Where(predicate);
    }

    public static IQueryable<T> IncludeProperties<T>(this IQueryable<T> query,
        params string[] includeProperties) where T : class
    {
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }
        return query;
    }

    public static IQueryable<T> AsNoTrackingIfReadOnly<T>(this IQueryable<T> query, bool isReadOnly = true)
        where T : class
    {
        return isReadOnly ? query.AsNoTracking() : query;
    }
}