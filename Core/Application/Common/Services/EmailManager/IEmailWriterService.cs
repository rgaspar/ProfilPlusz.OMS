using Application.Common.DTOs.Email;

namespace Application.Common.Services.EmailManager
{
    public interface IEmailWriterService
    {
        Task CreateDraftAsync(DraftEmailDto email, CancellationToken cancellationToken = default);
    }
}
