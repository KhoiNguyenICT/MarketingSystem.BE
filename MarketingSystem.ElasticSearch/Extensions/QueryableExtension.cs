using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MarketingSystem.ElasticSearch.Extensions
{
    public static class QueryableExtension
    {
        #region method

        internal static readonly MethodInfo IncludeMethodInfo
           = typeof(QueryableExtension)
               .GetTypeInfo().GetDeclaredMethods(nameof(SearchIncludeInternal))
               .FirstOrDefault();

        internal static readonly MethodInfo ThenIncludeMethodInfo
           = typeof(QueryableExtension)
               .GetTypeInfo().GetDeclaredMethods(nameof(SearchThenIncludeInternal))
               .FirstOrDefault();

        internal static readonly MethodInfo ThenIncludeMethodInfoEnumerable
          = typeof(QueryableExtension)
              .GetTypeInfo().GetDeclaredMethods(nameof(SearchThenIncludeEnumerable))
              .FirstOrDefault();

        internal static readonly MethodInfo ThenIncludeMethodInfoCollection
          = typeof(QueryableExtension)
              .GetTypeInfo().GetDeclaredMethods(nameof(SearchThenIncludeCollection))
              .FirstOrDefault();

        #endregion method

        #region FirstOrDefault

        public static Task<TEntity> FirstOrDefaultAsync<TEntity>(this IQueryable<TEntity> source, string propertyName, object value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");
            MemberExpression property = Expression.Property(parameter, propertyName);
            ConstantExpression rightSide = Expression.Constant(value);
            BinaryExpression operation = Expression.Equal(property, rightSide);
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), typeof(bool));
            Expression<Func<TEntity, bool>> predicate = (Expression<Func<TEntity, bool>>)Expression.Lambda(delegateType, operation, parameter);

            return source.FirstOrDefaultAsync(predicate);
        }

        #endregion FirstOrDefault

        #region Include

        public static IQueryable<TEntity> SearchInclude<TEntity>(this IQueryable<TEntity> source, LambdaExpression expression)
        {
            MethodInfo genericMethod = IncludeMethodInfo.MakeGenericMethod(typeof(TEntity), expression.Body.Type);
            return (IQueryable<TEntity>)genericMethod.Invoke(null, new object[] { source, expression });
        }

        private static IQueryable<TEntity> SearchIncludeInternal<TEntity, TProperty>(IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class
        {
            return source.Include(navigationPropertyPath);
        }

        #endregion Include

        #region ThenInclude

        public static IQueryable<TEntity> SearchThenInclude<TEntity>(this IQueryable<TEntity> source, LambdaExpression expression)
        {
            MethodInfo genericMethod;
            var typePreviousProperty = source.GetType().GetGenericArguments()[1];
            if (typePreviousProperty.GetInterfaces().Any(item => item.Name == nameof(ICollection)))
            {
                genericMethod = ThenIncludeMethodInfoCollection;
            }
            else if (typePreviousProperty.GetInterfaces().Any(item => item.Name == nameof(IEnumerable)))
            {
                genericMethod = ThenIncludeMethodInfoEnumerable;
            }
            else
            {
                genericMethod = ThenIncludeMethodInfo;
            }
            genericMethod = genericMethod.MakeGenericMethod(typeof(TEntity), expression.Parameters[0].Type, expression.Body.Type);
            return (IQueryable<TEntity>)genericMethod.Invoke(null, new object[] { source, expression });
        }

        private static IIncludableQueryable<TEntity, TProperty> SearchThenIncludeCollection<TEntity, TPreviousProperty, TProperty>(IIncludableQueryable<TEntity, ICollection<TPreviousProperty>> source, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath) where TEntity : class
        {
            return source.ThenInclude(navigationPropertyPath);
        }

        private static IIncludableQueryable<TEntity, TProperty> SearchThenIncludeEnumerable<TEntity, TPreviousProperty, TProperty>(IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>> source, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath) where TEntity : class
        {
            return source.ThenInclude(navigationPropertyPath);
        }

        private static IIncludableQueryable<TEntity, TProperty> SearchThenIncludeInternal<TEntity, TPreviousProperty, TProperty>(IIncludableQueryable<TEntity, TPreviousProperty> source, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath) where TEntity : class
        {
            return source.ThenInclude(navigationPropertyPath);
        }

        #endregion ThenInclude
    }
}