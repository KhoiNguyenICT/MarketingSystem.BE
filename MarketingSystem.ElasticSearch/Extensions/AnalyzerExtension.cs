using Nest;
using System;
using System.Linq.Expressions;

namespace MarketingSystem.ElasticSearch.Extensions
{
    public static class AnalyzerExtension
    {
        public static PropertiesDescriptor<T> UnicodeAnalyzer<T>(this PropertiesDescriptor<T> mappingDescriptor, Expression<Func<T, object>> selector, string analyzerName = "icu") where T : class
        {
            var uPropertyName = selector.GetUnicodeMemberName(true);
            return mappingDescriptor.Text(p => p.Name(selector).CopyTo(c => c.Field(uPropertyName)))
                .Text(p => p.Name(uPropertyName).Analyzer(analyzerName));
        }

        public static PropertiesDescriptor<T> UnicodeAnalyzer<T>(this PropertiesDescriptor<T> mappingDescriptor, string propertyName, string analyzerName = "icu") where T : class
        {
            var uPropertyName = propertyName.GetUnicodeMemberName();
            return mappingDescriptor.Text(p => p.Name(propertyName).CopyTo(c => c.Field(uPropertyName)))
                .Text(p => p.Name(uPropertyName).Analyzer(analyzerName));
        }
    }
}