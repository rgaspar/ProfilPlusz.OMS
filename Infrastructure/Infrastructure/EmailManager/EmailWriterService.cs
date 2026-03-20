using Application.Common.DTOs.Email;
using Application.Common.Services.EmailManager;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;

namespace Infrastructure.EmailManager
{
    public class EmailWriterService : IEmailWriterService
    {
        public async Task CreateDraftAsync(EmailDto email)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Saját Név", "Teszt01ProfilPlusz@gmail.com"));
            message.To.Add(new MailboxAddress("Címzett", "joni9103@outlook.com"));
            message.Subject = "Teszt draft email";

            message.Body = new TextPart("plain")
            {
                Text = "Ez egy draft email .NET-ből MailKit-tel."
            };

            using var client = new ImapClient();

            await client.ConnectAsync("imap.gmail.com", 993, true);

            // ⚠️ App Password kell!
            await client.AuthenticateAsync("Teszt01ProfilPlusz@gmail.com", "oqyv yabu wamh ugdm");

            // Gmail Drafts mappa
            var drafts = client.GetFolder(SpecialFolder.Drafts);
            await drafts.OpenAsync(FolderAccess.ReadWrite);

            // Draft mentése
            await drafts.AppendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}
