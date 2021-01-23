using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iSpindelBlazorWeb.Shared
{
    public static class Extensions
    {
        public static string ToJava(this bool b)
        {
            return b ? "true" : "false";
        }

        public static DateTime Trim(this DateTime date, long ticks) { return new DateTime(date.Ticks - (date.Ticks % ticks), date.Kind); }
        public static DateTime TrimToSecond(this DateTime date) { return date.Trim(TimeSpan.TicksPerSecond); }
        public static DateTime TrimToSecond(this DateTime date, int seconds) { return date.Trim(TimeSpan.TicksPerSecond * seconds); }
        public static DateTime TrimToMinute(this DateTime date) { return date.Trim(TimeSpan.TicksPerMinute); }
        public static DateTime TrimToMinute(this DateTime date, int minutes) { return date.Trim(TimeSpan.TicksPerMinute * minutes); }
        public static DateTime TrimToHour(this DateTime date) { return date.Trim(TimeSpan.TicksPerHour); }
        public static DateTime TrimToHour(this DateTime date, int minutes) { return date.Trim(TimeSpan.TicksPerHour * minutes); }

        public static string ToString(this decimal? d, string? format) 
        {
            return d.HasValue ? d.Value.ToString(format) : "";
        }

        public static IEnumerable<TEntity> OrderBy<TEntity>(this IEnumerable<TEntity> source, string orderByProperty, bool asc = true)
        {
            return asc
                ? source.OrderBy(x => x.GetType().GetProperty(orderByProperty)?.GetValue(x, null))
                : source.OrderByDescending(x => x.GetType().GetProperty(orderByProperty)?.GetValue(x, null));
        }

        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool asc = true)
        {
            var command = asc ? "OrderBy" : "OrderByDescending";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExpression));
            return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}
