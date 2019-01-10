using MarketingSystem.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MarketingSystem.Core
{
    public class QueryResult<T>
    {
        public long Count { get; set; }
        public IEnumerable<T> Items { get; set; }
    }

    public static class QueryResultExtension
    {
        #region ElasticSearchResult

        public static QueryResult<T> ToQueryResult<T>(this ISearchResponse<T> searchResponse) where T : class
        {
            return new QueryResult<T>()
            {
                Count = searchResponse.Total,
                Items = searchResponse.Documents
            };
        }

        #endregion ElasticSearchResult

        #region ToQueryResult<T>

        public static QueryResult<T> ToQueryResult<T>(
         this IEnumerable<T> queryable, long total)
        {
            return new QueryResult<T>
            {
                Count = total,
                Items = queryable
            };
        }

        public static async Task<QueryResult<T>> ToQueryResult<T>(
          this IQueryable<T> queryable)
        {
            var list = await queryable.ToListAsync();
            return new QueryResult<T>
            {
                Count = list.Count,
                Items = list
            };
        }

        public static async Task<QueryResult<T>> ToQueryResult<T>(
            this IQueryable<T> queryable,
            int skip,
            int take)
        {
            return new QueryResult<T>
            {
                Count = await queryable.CountAsync(),
                Items = await queryable.Skip(skip).Take(take).ToListAsync()
            };
        }

        public static async Task<QueryResult<T>> ToQueryResult<T>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            string[] sorts,
            Func<IQueryable<T>, string, IQueryable<T>> orderBy)
        {
            var sortedQuery = sorts.Reverse().Aggregate(queryable, orderBy);
            return new QueryResult<T>
            {
                Count = await queryable.CountAsync(),
                Items = await sortedQuery.Skip(skip).Take(take).ToListAsync()
            };
        }

        public static async Task<QueryResult<T>> ToQueryResult<T>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            SortDirection sortDirection,
            params Expression<Func<T, object>>[] sortExpressions)
        {
            return new QueryResult<T>
            {
                Count = await queryable.CountAsync(),
                Items = await queryable.SortBy(sortDirection, sortExpressions).Skip(skip).Take(take).ToListAsync()
            };
        }

        public static async Task<QueryResult<T>> ToQueryResult<T>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            SortDirection sortDirection,
            IEnumerable<Expression<Func<T, object>>> sortExpressions)
        {
            return await queryable.ToQueryResult(skip, take, sortDirection, sortExpressions?.ToArray());
        }

        public static async Task<QueryResult<T>> ToQueryResult<T>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, bool>> predicate)
        {
            return await queryable.Where(predicate).ToQueryResult(skip, take);
        }

        public static async Task<QueryResult<T>> ToQueryResult<T>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Func<IQueryable<T>, IQueryable<T>> include) where T : class
        {
            return await queryable.IncludeAll(include).ToQueryResult(skip, take);
        }

        public static async Task<QueryResult<T>> ToQueryResult<T>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> include) where T : class
        {
            return await queryable.Where(predicate).IncludeAll(include).ToQueryResult(skip, take);
        }

        public static async Task<QueryResult<T>> ToQueryResult<T>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> include,
            SortDirection sortDirection = SortDirection.Ascending,
            params Expression<Func<T, object>>[] sortExpressions) where T : class
        {
            return await queryable.Where(predicate).IncludeAll(include).ToQueryResult(skip, take, sortDirection, sortExpressions);
        }

        public static async Task<QueryResult<T>> ToQueryResult<T>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> include,
            SortDirection sortDirection,
            IEnumerable<Expression<Func<T, object>>> sortExpressions) where T : class
        {
            return await queryable.Where(predicate).IncludeAll(include).ToQueryResult(skip, take, sortDirection, sortExpressions);
        }

        #endregion ToQueryResult<T>

        #region ToQueryResult<T, TOut>

        public static async Task<QueryResult<TOut>> ToQueryResult<T, TOut>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, TOut>> selector)
        {
            return new QueryResult<TOut>
            {
                Count = await queryable.CountAsync(),
                Items = await queryable.Select(selector).Skip(skip).Take(take).ToListAsync()
            };
        }

        public static async Task<QueryResult<TOut>> ToQueryResult<T, TOut>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, TOut>> selector,
            SortDirection sortDirection,
            params Expression<Func<T, object>>[] sortExpressions)
        {
            return new QueryResult<TOut>
            {
                Count = await queryable.CountAsync(),
                Items = await queryable.SortBy(sortDirection, sortExpressions).Select(selector).Skip(skip).Take(take).ToListAsync()
            };
        }

        public static async Task<QueryResult<TOut>> ToQueryResult<T, TOut>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, TOut>> selector,
            SortDirection sortDirection,
            IEnumerable<Expression<Func<T, object>>> sortExpressions)
        {
            return await queryable.ToQueryResult(skip, take, selector, sortDirection, sortExpressions?.ToArray());
        }

        public static async Task<QueryResult<TOut>> ToQueryResult<T, TOut>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, TOut>> selector,
            Expression<Func<T, bool>> predicate)
        {
            return await queryable.Where(predicate).ToQueryResult(skip, take, selector);
        }

        public static async Task<QueryResult<TOut>> ToQueryResult<T, TOut>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, TOut>> selector,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include) where T : class
        {
            return await queryable.IncludeAll(include).ToQueryResult(skip, take, selector);
        }

        public static async Task<QueryResult<TOut>> ToQueryResult<T, TOut>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, TOut>> selector,
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include) where T : class
        {
            return await queryable.Where(predicate).IncludeAll(include).ToQueryResult(skip, take, selector);
        }

        public static async Task<QueryResult<TOut>> ToQueryResult<T, TOut>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, TOut>> selector,
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include,
            SortDirection sortDirection = SortDirection.Ascending,
            params Expression<Func<T, object>>[] sortExpressions) where T : class
        {
            return await queryable.Where(predicate).IncludeAll(include).ToQueryResult(skip, take, selector, sortDirection, sortExpressions);
        }

        public static async Task<QueryResult<TOut>> ToQueryResult<T, TOut>(
            this IQueryable<T> queryable,
            int skip,
            int take,
            Expression<Func<T, TOut>> selector,
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include,
            SortDirection sortDirection,
            IEnumerable<Expression<Func<T, object>>> sortExpressions) where T : class
        {
            return await queryable.Where(predicate).IncludeAll(include).ToQueryResult(skip, take, selector, sortDirection, sortExpressions?.ToArray());
        }

        #endregion ToQueryResult<T, TOut>
    }
}