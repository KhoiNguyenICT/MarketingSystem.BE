using MarketingSystem.ElasticSearch.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MarketingSystem.ElasticSearch
{
    internal class IncludableBuilder<TEntity, TProperty> : IIncludableBuilder<TEntity, TProperty>
    {
        public Dictionary<LambdaExpression, TypeExpression> IncludeExpression { get; private set; }

        public IncludableBuilder(Dictionary<LambdaExpression, TypeExpression> includeExpression)
        {
            IncludeExpression = includeExpression;
        }
    }

    public static class IncludableBuilderExtension
    {
        public static IIncludableBuilder<TEntity, TProperty> Include<TEntity, TProperty>(this IIncludableQueryBuilder<TEntity> builder, Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        {
            builder.IncludeExpression.Add(navigationPropertyPath, TypeExpression.Include);
            return new IncludableBuilder<TEntity, TProperty>(builder.IncludeExpression);
        }

        public static IIncludableBuilder<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IIncludableBuilder<TEntity, TPreviousProperty> builder,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        {
            builder.IncludeExpression.Add(navigationPropertyPath, TypeExpression.ThenInclude);
            return new IncludableBuilder<TEntity, TProperty>(builder.IncludeExpression);
        }

        public static IIncludableBuilder<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IIncludableBuilder<TEntity, IEnumerable<TPreviousProperty>> builder,
           Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        {
            builder.IncludeExpression.Add(navigationPropertyPath, TypeExpression.ThenInclude);
            return new IncludableBuilder<TEntity, TProperty>(builder.IncludeExpression);
        }

        public static IIncludableBuilder<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IIncludableBuilder<TEntity, ICollection<TPreviousProperty>> builder,
           Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        {
            builder.IncludeExpression.Add(navigationPropertyPath, TypeExpression.ThenInclude);
            return new IncludableBuilder<TEntity, TProperty>(builder.IncludeExpression);
        }
    }
}