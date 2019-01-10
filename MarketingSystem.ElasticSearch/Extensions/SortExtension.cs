using Nest;
using System.Collections.Generic;
using System.Linq;

namespace MarketingSystem.ElasticSearch.Extensions
{
    public static class SortExtension
    {
        public static IEnumerable<ISort> GetSorts(this string[] sorts)
        {
            return sorts?.Length == 0 ? null : sorts?.Select(s => s.GetSort()).Where(s => s != null);
        }

        public static SortDescriptor<T> GetSortDescriptor<T>(this string[] sorts) where T : class
        {
            sorts = sorts.Where(sort => !string.IsNullOrEmpty(sort)).ToArray();
            if (sorts?.Length == 0)
                return null;

            return sorts.Aggregate(new SortDescriptor<T>(), (current, item) =>
                item.StartsWith('-') ?
                    current.Field(f => f.Field(item.Substring(1)).Order(SortOrder.Descending)) :
                    current.Field(f => f.Field(item).Order(SortOrder.Ascending)));
        }

        public static ISort GetSort(this string sort)
        {
            return string.IsNullOrEmpty(sort) ? null : new SortField()
            {
                Field = sort.Replace("-", ""),
                Order = sort.StartsWith("-") ? SortOrder.Descending : SortOrder.Ascending
            };
        }
    }
}