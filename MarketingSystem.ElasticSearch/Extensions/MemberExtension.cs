using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace MarketingSystem.ElasticSearch.Extensions
{
    public static class MemberExtension
    {
        public static string GetMemberName<T, TProperty>(this Expression<Func<T, TProperty>> property, bool toCameCase = false)
        {
            MemberExpression memberExpression;
            if (property.Body is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)(property.Body);
                memberExpression = (MemberExpression)(unaryExpression.Operand);
            }
            else
            {
                memberExpression = (MemberExpression)(property.Body);
            }

            return toCameCase ? memberExpression.Member.Name.ToCameCase() : memberExpression.Member.Name;
        }

        public static string GetUnicodeMemberName<T>(this Expression<Func<T, object>> property, bool toCameCase = false)
        {
            return $"u_{property.GetMemberName(toCameCase)}";
        }

        public static string GetUnicodeMemberName(this string propertyName)
        {
            return $"u_{propertyName}";
        }

        public static IEnumerable<string> GetUnicodeMemberNames(this string[] propertyNames)
        {
            return propertyNames.Select(p => p.GetUnicodeMemberName());
        }

        public static IEnumerable<string> GetMemberNames<T>(this Expression<Func<T, object>>[] properties, bool toCameCase = false)
        {
            return properties.Select(p => p.GetMemberName(toCameCase));
        }

        public static IEnumerable<string> GetUnicodeMemberNames<T>(this Expression<Func<T, object>>[] properties, bool toCameCase = false)
        {
            return properties.Select(p => p.GetUnicodeMemberName(toCameCase));
        }
    }
}