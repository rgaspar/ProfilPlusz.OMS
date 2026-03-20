using Application.Common.DTOs.Partner;

namespace Application.Common.Services.PartnerManager
{
    public interface IPartnerService
    {
        Task<List<PartnerDto>> SearchAsync(string search);
    }
}
