using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using dotnetmud.Web;
using System.Linq.Expressions;

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extensions to help with DataTables.AspNet packages.
/// </summary>
public static class DataTablesExtensions
{
    /// <summary>
    /// Registers the appropriate services for DataTables.AspNet.
    /// </summary>
    /// <param name="services">The services collection to modify</param>
    /// <returns>The modified services collection</returns>
    public static IServiceCollection AddDataTables(this IServiceCollection services)
    {
        services.RegisterDataTables();
        return services;
    }

    public static IQueryable<T> GetPage<T>(this IQueryable<T> source, IDataTablesRequest request)
        => source.Skip(request.Start).Take(request.Length);

    public static IQueryable<T> GlobalFilterBy<T>(this IQueryable<T> source, ISearch? search, IEnumerable<IColumn>? columns)
    {
        if (string.IsNullOrWhiteSpace(search?.Value))
        {
            return source;
        }

        IEnumerable<IColumn> searchableColumns = columns?.Where(x => x.IsSearchable) ?? [];
        if (!searchableColumns.Any())
        {
            return source;
        }

        Expression? predicateBody = null;
        var parameter = Expression.Parameter(typeof(T), "x");

        foreach (var column in searchableColumns)
        {
            var selector = Expression.PropertyOrField(parameter, column.Field);
            var left = Expression.NotEqual(selector, Expression.Default(selector.Type));
            var toString = Expression.Call(selector, selector.Type.GetMethod("ToString", Type.EmptyTypes)!);
            var indexOf = Expression.Call(
                toString,
                typeof(string).GetMethod("IndexOf", [typeof(string), typeof(StringComparison)])!,
                Expression.Constant(search.Value),
                Expression.Constant(StringComparison.InvariantCultureIgnoreCase)
            );
            var right = Expression.GreaterThanOrEqual(indexOf, Expression.Constant(0));
            var andExpression = Expression.AndAlso(left, right);
            predicateBody = predicateBody == null ? andExpression : Expression.OrElse(predicateBody, andExpression);
        }

        if (predicateBody is null)
        {
            return source;
        }

        var lambda = Expression.Lambda<Func<T, bool>>(predicateBody, parameter);
        var whereCallExpression = Expression.Call(typeof(Queryable), "Where", [source.ElementType], source.Expression, lambda);
        return source.Provider.CreateQuery<T>(whereCallExpression);
    }


    public static IQueryable<T> SortBy<T>(this IQueryable<T> source, IEnumerable<IColumn> columns, IDictionary<string, string> mapping)
    {
        Expression expression = source.Expression;
        bool firstTime = true;

        var sortedColumns = columns.Where(x => x.IsSortable && x.Sort is not null).OrderBy(x => x.Sort.Order);

        foreach (var sortedColumn in sortedColumns)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            string fieldName = mapping[sortedColumn.Field];
            var selector = Expression.PropertyOrField(parameter, fieldName);
            var lambda = Expression.Lambda(selector, parameter);
            var method = sortedColumn.Sort.Direction == SortDirection.Descending
                ? firstTime ? "OrderByDescending" : "ThenByDescending"
                : firstTime ? "OrderBy" : "ThenBy";
            expression = Expression.Call(typeof(Queryable), method, [source.ElementType, selector.Type], expression, Expression.Quote(lambda));
            firstTime = false;
        }
        return firstTime ? source : source.Provider.CreateQuery<T>(expression);
    }
}
