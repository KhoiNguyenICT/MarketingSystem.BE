using AutoMapper;
using MarketingSystem.Model.Entities;
using MarketingSystem.Service.Dtos.LoginHistory;

namespace MarketingSystem.Service.Mapper
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<Asset, Asset>();
        }
    }
}