using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IPartnerRepository
    {
        Task<List<Partner>> GetByCountiesAsync(IEnumerable<string> counties, CancellationToken cancellationToken);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
