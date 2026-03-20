using Application.Common.DTOs.Location;
using Application.Common.DTOs.Partner;
using AutoMapper;
using Domain.Common.Location;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddressInfoDto, AddressInfo>();
            CreateMap<AddressInfo, AddressInfoDto>();
            CreateMap<Partner, PartnerDto>();
        }
    }
}
