using Application.Common.DTOs.Email;

namespace Application.Common.Services.EmailManager
{
    public interface IEmailReaderService
    {
        Task<List<EmailDto>> GetUnreadEmailsAsync();
    }
}
