using Application.Common.DTOs.Partner;
using Application.Common.Repositories;
using AutoMapper;

namespace Application.Common.Services.PartnerManager
{
    public class PartnerService
    {
        private readonly IPartnerRepository _repo;
        private readonly IMapper _mapper;

        public PartnerService(IPartnerRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<PartnerDto>> SearchAsync(string search)
        {
            var partners = await _repo.SearchAsync(search);
            return _mapper.Map<List<PartnerDto>>(partners);
        }
    }
}
