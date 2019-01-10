using MarketingSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MarketingSystem.Core
{
    public static class LinqExtensions
    {
        public static Func<T, object> GetLambda<T>(string property)
        {
            var param = Expression.Parameter(typeof(T), "p");

            Expression parent = Expression.Property(param, property);
            var isValueType = parent.Type.GetTypeInfo().IsValueType;
            if (isValueType)
            {
                return Expression.Lambda<Func<T, object>>(parent, param).Compile();
            }
            var convert = Expression.Convert(parent, typeof(object));
            return Expression.Lambda<Func<T, object>>(convert, param).Compile();
        }

        public static Expression<T> ComposeExpression<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            if (first.IsEquals(second)) return first;
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> Compose<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second,
            Func<Expression, Expression, Expression> merge)
        {
            if (first.IsEquals(t => true)) return second;
            if (second.IsEquals(t => true)) return first;
            return ComposeExpression(first, second, merge);
        }

        private static LambdaExpression GenerateSelector<T>(string propertyName, out Type resultType) where T : class
        {
            // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField).
            var parameter = Expression.Parameter(typeof(T), "Entity");
            //  create the selector part, but support child properties
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields.
                string[] childProperties = propertyName.Split('.');
                property = typeof(T).GetProperty(childProperties[0], BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
                for (int i = 1; i < childProperties.Length; i++)
                {
                    property = property.PropertyType.GetProperty(childProperties[i], BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }
            resultType = property.PropertyType;
            // Create the order by expression.
            return Expression.Lambda(propertyAccess, parameter);
        }

        private static MethodCallExpression GenerateMethodCall<T>(IQueryable<T> source, string methodName, string fieldName) where T : class
        {
            var type = typeof(T);
            var selector = GenerateSelector<T>(fieldName, out var selectorResultType);
            var resultExp = Expression.Call(typeof(Queryable), methodName,
                            new[] { type, selectorResultType },
                            source.Expression, Expression.Quote(selector));
            return resultExp;
        }

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (second == null) return first;
            return first.Compose(second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, params Expression<Func<T, bool>>[] seconds)
        {
            if (seconds == null || seconds.All(t => t == null)) return first;
            return seconds.Where(t => t != null).Aggregate(first, (current, expression) => current.AndAlso(expression));
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (second == null) return first;
            return first.Compose(second, Expression.OrElse);
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> first, params Expression<Func<T, bool>>[] seconds)
        {
            if (seconds == null || seconds.All(t => t == null)) return first;
            return seconds.Where(t => t != null).Aggregate(first, (current, expression) => current.OrElse(expression));
        }

        public static IQueryable<T> SortBy<T>(this IQueryable<T> query, SortDirection sortDirection, params Expression<Func<T, object>>[] sortExpressions)
        {
            if (sortExpressions == null || sortExpressions.All(t => t == null)) return query;
            IOrderedQueryable<T> orderedQuery = null;
            for (var i = 0; i < sortExpressions.Length; i++)
            {
                var sortExpItem = sortExpressions[i];
                if (sortExpItem == null) continue;
                if (sortDirection == SortDirection.Descending)
                {
                    orderedQuery = i == 0 ? query.OrderByDescending(sortExpItem) : orderedQuery.ThenByDescending(sortExpItem);
                }
                else
                {
                    orderedQuery = i == 0 ? query.OrderBy(sortExpItem) : orderedQuery.ThenBy(sortExpItem);
                }
            }
            return orderedQuery ?? query;
        }

        public static IQueryable<T> SortBy<T>(this IQueryable<T> query, SortDirection sortDirection, IEnumerable<Expression<Func<T, object>>> sortExpressions)
        {
            return query.SortBy(sortDirection, sortExpressions?.ToArray());
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string fieldName) where T : class
        {
            MethodCallExpression resultExp = GenerateMethodCall(source, "OrderBy", fieldName);
            return source.Provider.CreateQuery<T>(resultExp) as IOrderedQueryable<T>;
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string fieldName) where T : class
        {
            MethodCallExpression resultExp = GenerateMethodCall(source, "OrderByDescending", fieldName);
            return source.Provider.CreateQuery<T>(resultExp) as IOrderedQueryable<T>;
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string fieldName) where T : class
        {
            MethodCallExpression resultExp = GenerateMethodCall(source, "ThenBy", fieldName);
            return source.Provider.CreateQuery<T>(resultExp) as IOrderedQueryable<T>;
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string fieldName) where T : class
        {
            MethodCallExpression resultExp = GenerateMethodCall(source, "ThenByDescending", fieldName);
            return source.Provider.CreateQuery<T>(resultExp) as IOrderedQueryable<T>;
        }

        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, params string[] sortExpression) where T : class
        {
            if (sortExpression.Length == 0)
                return source;

            IOrderedQueryable<T> result = null;
            for (int index = 0; index < sortExpression.Length; index++)
            {
                var sortField = Regex.Replace(sortExpression[index], @"[\+\-]", string.Empty);
                if (sortExpression[index].StartsWith("-"))
                {
                    result = index == 0 ? source.OrderByDescending(sortField) : result.ThenByDescending(sortField);
                }
                else
                {
                    result = index == 0 ? source.OrderBy(sortField) : result.ThenBy(sortField);
                }
            }
            return result;
        }

        public static IQueryable<T> IncludeAll<T>(this IQueryable<T> queryable, Func<IQueryable<T>, IQueryable<T>> include) where T : class
        {
            if (include == null) return queryable;
            return include(queryable);
        }
    }

    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (_map.TryGetValue(p, out ParameterExpression replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
}