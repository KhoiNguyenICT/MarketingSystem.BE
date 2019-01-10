namespace MarketingSystem.ElasticSearch.Expressions
{
    public static class QueryExpression
    {
        public static BuildQuery<TEntity> BuildQuery<TEntity>()
        {
            return new BuildQuery<TEntity>();
        }
    }
}