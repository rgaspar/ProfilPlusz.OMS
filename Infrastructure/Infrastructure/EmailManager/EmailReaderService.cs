using Application.Common.DTOs.Email;
using Application.Common.Services.EmailManager;
using MailKit;
using MailKit.Net.Imap;

namespace Infrastructure.EmailManager
{
    public class EmailReaderService : IEmailReaderService
    {
        public async Task<List<EmailDto>> GetLatestEmailsAsync(int count)
        {
            using var client = new ImapClient();

            await client.ConnectAsync("imap.gmail.com", 993, true);
            await client.AuthenticateAsync("teszt01profilplusz@gmail.com", "oqyv yabu wamh ugdm");

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly);

            var emails = new List<EmailDto>();

            for (int i = inbox.Count - count; i < inbox.Count; i++)
            {
                var message = await inbox.GetMessageAsync(i);

                emails.Add(new EmailDto
                {
                    Subject = message.Subject,
                    Body = message.TextBody,
                    From = message.From.ToString(),
                    Date = message.Date.DateTime
                });
            }

            await client.DisconnectAsync(true);

            return emails;
        }
    }
}
