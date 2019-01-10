using MarketingSystem.Model;
using MarketingSystem.Model.Entities;
using MarketingSystem.Service.Interfaces;

namespace MarketingSystem.Service.Implementations
{
    public class UrlTrackingService: BaseService<UrlTracking>, IUrlTrackingService
    {
        public UrlTrackingService(AppDbContext context) : base(context)
        {
        }
    }
}