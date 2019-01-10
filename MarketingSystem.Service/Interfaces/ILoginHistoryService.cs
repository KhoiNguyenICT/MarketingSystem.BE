using System.Threading.Tasks;
using MarketingSystem.Model.Entities;
using MarketingSystem.Service.Dtos.LoginHistory;

namespace MarketingSystem.Service.Interfaces
{
    public interface ILoginHistoryService: IService<LoginHistory>
    {
        bool CheckTokenLoginNeareast(CheckTokenLoginNeareastDto checkTokenLoginNeareastDto);
    }
}