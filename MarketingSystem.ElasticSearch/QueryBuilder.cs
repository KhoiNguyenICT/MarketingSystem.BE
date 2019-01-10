using MarketingSystem.ElasticSearch.Extensions;
using Nest;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace MarketingSystem.ElasticSearch
{
    public static class QueryBuilder
    {
        public static QueryContainer StartWith<T>(Expression<Func<T, object>>[] searchFields, string searchQuery, int boost = 1, Operator defaultOperator = Operator.And) where T : class
        {
            return new QueryStringQuery()
            {
                Fields = searchFields,
                Query = $"{searchQuery}*",
                Boost = boost,
                DefaultOperator = defaultOperator
            };
        }

        public static QueryContainer StartWith(string[] searchFields, string searchQuery, int boost = 1, Operator defaultOperator = Operator.And)
        {
            return new QueryStringQuery()
            {
                Fields = searchFields,
                Query = $"{searchQuery}*",
                Boost = boost,
                DefaultOperator = defaultOperator
            };
        }

        public static QueryContainer Contains<T>(Expression<Func<T, object>>[] searchFields, string searchQuery, int boost = 1, Operator defaultOperator = Operator.And) where T : class
        {
            return new QueryStringQuery()
            {
                Fields = searchFields,
                Query = $"*{searchQuery}*",
                Boost = boost,
                DefaultOperator = defaultOperator
            };
        }

        public static QueryContainer Contains(string[] searchFields, string searchQuery, int boost = 1, Operator defaultOperator = Operator.And)
        {
            return new QueryStringQuery()
            {
                Fields = searchFields,
                Query = $"*{searchQuery}*",
                Boost = boost,
                DefaultOperator = defaultOperator
            };
        }

        public static QueryContainer FullTextSearch(string[] searchFields, string searchQuery, Operator defaultOperator = Operator.And)
        {
            var queryContainer = StartWith(searchFields, searchQuery, 4);
            queryContainer |= Contains(searchFields, searchQuery, 3);
            queryContainer |= StartWith(searchFields.GetUnicodeMemberNames().ToArray(), searchQuery, 2);
            queryContainer |= Contains(searchFields.GetUnicodeMemberNames().ToArray(), searchQuery);
            return queryContainer;
        }

        public static QueryContainer FullTextSearch<T>(Expression<Func<T, object>>[] searchFields, string searchQuery, Operator defaultOperator = Operator.And) where T : class
        {
            var queryContainer = StartWith(searchFields, searchQuery, 4);
            queryContainer |= Contains(searchFields, searchQuery, 3);
            queryContainer |= StartWith(searchFields.GetUnicodeMemberNames(true).ToArray(), searchQuery, 2);
            queryContainer |= Contains(searchFields.GetUnicodeMemberNames(true).ToArray(), searchQuery);
            return queryContainer;
        }
    }
}