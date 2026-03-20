using Application.Common.DTOs.Email;

namespace Application.Common.Services.EmailManager
{
    public interface IEmailWriterService
    {
        Task CreateDraftAsync(EmailDto email);
    }
}
