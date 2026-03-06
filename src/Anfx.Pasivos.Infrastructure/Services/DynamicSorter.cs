using Anfx.Pasivos.Application.Common.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace Anfx.Pasivos.Infrastructure.Services;

public sealed class DynamicSorter : IDynamicSorter
{
    public IQueryable<T> ApplySort<T>(
        IQueryable<T> query,
        string sortColumn,
        bool descending = false)
    {
        if (string.IsNullOrWhiteSpace(sortColumn) || !IsValidProperty<T>(sortColumn))
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, sortColumn);
        var lambda = Expression.Lambda<Func<T, object>>(
            Expression.Convert(property, typeof(object)),
            parameter
        );

        return descending
            ? query.OrderByDescending(lambda)
            : query.OrderBy(lambda);
    }

    public IQueryable<T> ApplySort<T>(
        IQueryable<T> query,
        Dictionary<string, bool> sortColumns)
    {
        var orderedQuery = query;
        bool firstSort = true;

        foreach (var sort in sortColumns)
        {
            if (!IsValidProperty<T>(sort.Key)) continue;

            orderedQuery = firstSort
                ? ApplySort(orderedQuery, sort.Key, sort.Value)
                : ApplyThenBy((IOrderedQueryable<T>)orderedQuery, sort.Key, sort.Value);

            firstSort = false;
        }

        return orderedQuery;
    }

    private IOrderedQueryable<T> ApplyThenBy<T>(
        IOrderedQueryable<T> query,
        string sortColumn,
        bool descending)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, sortColumn);
        var lambda = Expression.Lambda<Func<T, object>>(
            Expression.Convert(property, typeof(object)),
            parameter
        );

        return descending
            ? query.ThenByDescending(lambda)
            : query.ThenBy(lambda);
    }

    private static bool IsValidProperty<T>(string propertyName)
        => typeof(T).GetProperty(
            propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
        ) != null;
}