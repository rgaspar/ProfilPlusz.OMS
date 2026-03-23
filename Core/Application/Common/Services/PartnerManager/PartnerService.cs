using Application.Common.DTOs.Partner;
using Application.Common.Repositories;
using AutoMapper;
using System.Diagnostics.Metrics;

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

        public async Task<List<PartnerDto>> GetRecommendedPartnersAsync(List<string> counties, CancellationToken cancellationToken)
        {
            var partners = await _repo.GetByCountiesAsync(counties, cancellationToken);

            return _mapper.Map<List<PartnerDto>>(partners);
        }
    }
}
