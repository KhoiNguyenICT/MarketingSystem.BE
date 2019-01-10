using System;

namespace MarketingSystem.Service.Dtos.Account
{
    public class AccountsDto: BaseDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string FullNameAcronym { get; set; }
    }
}