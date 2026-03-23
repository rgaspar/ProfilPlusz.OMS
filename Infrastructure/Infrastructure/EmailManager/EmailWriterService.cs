using Application.Common.DTOs.Email;
using Application.Common.Services.EmailManager;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;

namespace Infrastructure.EmailManager
{
    public class EmailWriterService : IEmailWriterService
    {
        public async Task CreateDraftAsync(EmailDto email, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("ProfilPlusz", "Teszt01ProfilPlusz@gmail.com"));

            message.To.Add(new MailboxAddress("Címzett neve", "joni9103@outlook.com"));

            message.Subject = email.Subject;

            message.Body = new TextPart("html")
            {
                Text = email.Body
            };

            using var client = new ImapClient();

            await client.ConnectAsync(
                "imap.gmail.com",
                993,
                true,
                cancellationToken);

            await client.AuthenticateAsync(
                "Teszt01ProfilPlusz@gmail.com",
                "oqyv yabu wamh ugdm",
                cancellationToken);

            var drafts = client.GetFolder(SpecialFolder.Drafts);

            await drafts.OpenAsync(
                FolderAccess.ReadWrite,
                cancellationToken);

            await drafts.AppendAsync(
                        new AppendRequest(message),
                        cancellationToken);

            await client.DisconnectAsync(
                true,
                cancellationToken);
        }
    }
}
