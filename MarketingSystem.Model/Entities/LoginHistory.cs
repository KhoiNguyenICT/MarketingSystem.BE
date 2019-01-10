namespace MarketingSystem.Model.Entities
{
    public class LoginHistory: BaseEntity
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}