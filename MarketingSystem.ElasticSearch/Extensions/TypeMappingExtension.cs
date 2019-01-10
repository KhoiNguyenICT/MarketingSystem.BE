using Nest;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MarketingSystem.ElasticSearch.Extensions
{
    public static class TypeMappingExtension
    {
        public static TypeMappingDescriptor<TSource> MapFullText<TSource, TResult>(this TypeMappingDescriptor<TSource> typeMapping, Expression<Func<TSource, TResult>> selectorUnicodeFields) where TSource : class
        {
            if (selectorUnicodeFields.Body is NewExpression)
            {
                var expression = (NewExpression)selectorUnicodeFields.Body;
                foreach (var member in expression.Members)
                {
                    SetUnicodeAnalyzer(typeMapping, member);
                }
            }
            else if (selectorUnicodeFields.Body is MemberExpression)
            {
                var expression = (MemberExpression)selectorUnicodeFields.Body;
                SetUnicodeAnalyzer(typeMapping, expression.Member);
            }
            return typeMapping;
        }

        public static ClrTypeMappingDescriptor<TSource> RenameAll<TSource>(this ClrTypeMappingDescriptor<TSource> typeMappingDescriptor) where TSource : class
        {
            var typeMap = (IClrTypeMapping<TSource>)typeMappingDescriptor;

            var properties = typeof(TSource).GetProperties();
            if (typeMap.TypeName == null)
                throw new Exception($"{typeof(TSource).FullName} don't have type name");

            foreach (var property in properties)
            {
                var expression = GetExpressionProperty<TSource>(property);
                typeMappingDescriptor = typeMappingDescriptor.Rename(expression, $"{typeMap.TypeName}_{expression.GetMemberName(true)}");
            }

            return typeMappingDescriptor;
        }

        private static void SetUnicodeAnalyzer<TSource>(TypeMappingDescriptor<TSource> typeMapping, MemberInfo member) where TSource : class
        {
            var sourceType = typeof(TSource);
            var property = sourceType.GetProperty(member.Name);

            Expression.Parameter(typeof(TSource));
            var selector = GetExpressionProperty<TSource>(property);
            typeMapping.Properties(p => p.UnicodeAnalyzer(selector));
        }

        private static Expression<Func<TSource, object>> GetExpressionProperty<TSource>(PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof(TSource));
            var expr = Expression.Property(parameter, property);
            return Expression.Lambda<Func<TSource, object>>(Expression.Convert(expr, typeof(object)), parameter);
        }
    }
}