using Application.Common.DTOs.Email;
using Application.Common.Services.EmailManager;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Options;

namespace Infrastructure.EmailManager
{
    public class EmailReaderService : IEmailReaderService
    {
        private readonly ImapSettings _settings;

        public EmailReaderService(IOptions<EmailServiceSettings> settings)
        {
            _settings = settings.Value.Imap;
        }

        public async Task<List<EmailDto>> GetUnreadEmailsAsync()
        {
            using var client = new ImapClient();

            await client.ConnectAsync(_settings.Host, _settings.Port, true);

            await client.AuthenticateAsync(_settings.UserName, _settings.Password);

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadWrite);

            SearchQuery subjectQuery = null;

            foreach (var subject in _settings.SubjectFilters)
            {
                var q = SearchQuery.SubjectContains(subject);

                subjectQuery = subjectQuery == null
                    ? q
                    : subjectQuery.Or(q);
            }

            // olvasatlan + subject filter
            var query = SearchQuery.NotSeen.And(subjectQuery);

            var uids = await inbox.SearchAsync(query);

            var latest = uids.Reverse();

            var emails = new List<EmailDto>();

            foreach (var uid in latest)
            {
                var message = await inbox.GetMessageAsync(uid);

                emails.Add(new EmailDto
                {
                    ExternalId = uid.ToString(),
                    Subject = message.Subject,
                    Body = message.TextBody,
                    From = message.From.ToString(),
                    ReceivedAt = message.Date.LocalDateTime,
                    
                });

                // olvasottnak jelölés
                await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
            }

            await client.DisconnectAsync(true);

            return emails;
        }
    }
}
