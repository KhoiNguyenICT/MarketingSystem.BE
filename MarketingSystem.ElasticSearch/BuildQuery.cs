using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MarketingSystem.ElasticSearch.Contracts;

namespace MarketingSystem.ElasticSearch
{
    public class BuildQuery<TEntity> : IBuildQuery<TEntity>
    {
        public Dictionary<LambdaExpression, TypeExpression> IncludeExpression { get; internal set; }

        public BuildQuery()
        {
            IncludeExpression = new Dictionary<LambdaExpression, TypeExpression>();
        }

        public IIncludableBuilder<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        {
            IncludeExpression.Add(navigationPropertyPath, TypeExpression.Include);
            return new IncludableBuilder<TEntity, TProperty>(IncludeExpression);
        }
    }
}