namespace MarketingSystem.Service.Dtos.Token
{
    public class TokenDto
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
}