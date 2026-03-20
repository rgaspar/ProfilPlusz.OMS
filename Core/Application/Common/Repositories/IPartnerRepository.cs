using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IPartnerRepository
    {
        Task<List<Partner>> SearchAsync(string search);
    }
}
