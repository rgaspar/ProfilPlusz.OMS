using Domain.Services.Email.Models;

namespace Domain.Services.Email
{
    public interface IEmailParserService
    {
        ParsedEmail Parse(string emailBody);
    }
}
