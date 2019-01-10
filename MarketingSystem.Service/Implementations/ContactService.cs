using MarketingSystem.Model;
using MarketingSystem.Model.Entities;
using MarketingSystem.Service.Interfaces;

namespace MarketingSystem.Service.Implementations
{
    public class ContactService: BaseService<Contact>, IContactService
    {
        public ContactService(AppDbContext context) : base(context)
        {
        }
    }
}