using MarketingSystem.Model;
using MarketingSystem.Model.Entities;
using MarketingSystem.Service.Interfaces;

namespace MarketingSystem.Service.Implementations
{
    public class DomainService: BaseService<Domain>, IDomainService
    {
        public DomainService(AppDbContext context) : base(context)
        {
        }
    }
}