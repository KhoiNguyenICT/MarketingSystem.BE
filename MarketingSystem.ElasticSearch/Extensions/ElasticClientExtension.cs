using System;
using Nest;

namespace MarketingSystem.ElasticSearch.Extensions
{
    public static class ConnectionSettingsExtension
    {
        public static IElasticClient AutoRefeshDefault(this IElasticClient client, bool value)
        {
            MapTypeSearch.AutoRefeshDefault = value;
            return client;
        }

        public static DocumentInfo<TEntity> Mapping<TEntity>(this IElasticClient client, Func<DocumentInfo<TEntity>, DocumentInfo<TEntity>> documentInfo) where TEntity : class
        {
            var doc = documentInfo(new DocumentInfo<TEntity>());
            if (string.IsNullOrEmpty(doc.Index))
                doc.SetIndex(client.ConnectionSettings.DefaultIndex);

            if (string.IsNullOrEmpty(doc.Type))
            {
                var resolver = new TypeNameResolver(client.ConnectionSettings);
                doc.SetType(resolver.Resolve<TEntity>());
            }

            MapTypeSearch.AddMap<TEntity>(doc);
            return doc;
        }

        public static DocumentInfo<TEntity> Mapping<TEntity, TEntityTarget>(this IElasticClient client, Func<DocumentInfo<TEntity>, DocumentInfo<TEntity>> documentInfo) where TEntity : class
            where TEntityTarget : class
        {
            var doc = documentInfo(new DocumentInfo<TEntity>());
            if (string.IsNullOrEmpty(doc.Index))
                doc.SetIndex(client.ConnectionSettings.DefaultIndex);

            if (string.IsNullOrEmpty(doc.Type))
            {
                var resolver = new TypeNameResolver(client.ConnectionSettings);
                doc.SetType(resolver.Resolve<TEntityTarget>());
            }
            doc.SetEntityTarget<TEntityTarget>();
            MapTypeSearch.AddMap<TEntity>(doc);
            return doc;
        }
    }
}