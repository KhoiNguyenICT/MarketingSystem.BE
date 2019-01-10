namespace MarketingSystem.ElasticSearch.Tracker
{
    public class TrackedPropertyEntry
    {
        public string Name { get; set; }
        public object OriginalValue { get; set; }
        public object CurrentValue { get; set; }
    }
}