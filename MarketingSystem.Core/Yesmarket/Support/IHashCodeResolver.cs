namespace MarketingSystem.Core.Yesmarket.Support
{
    internal interface IHashCodeResolver<in T>
    {
        int GetHashCodeFor(T obj);
    }
}