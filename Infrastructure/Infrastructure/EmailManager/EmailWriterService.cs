using Application.Common.DTOs.Email;
using Application.Common.Services.EmailManager;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.EmailManager
{
    public class EmailWriterService : IEmailWriterService
    {
        private readonly ImapSettings _imapSettings;
        private readonly SmtpSettings _smtpSettings;

        public EmailWriterService(IOptions<EmailServiceSettings> settings)
        {
            _imapSettings = settings.Value.Imap;
            _smtpSettings = settings.Value.Smtp;
        }

        public async Task CreateDraftAsync(DraftEmailDto email, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromAddress));

            message.To.Add(new MailboxAddress(email.ToName, email.ToEmail));

            message.Subject = email.Subject;

            message.Body = new TextPart("html")
            {
                Text = email.Body
            };

            using var client = new ImapClient();

            await client.ConnectAsync(_imapSettings.Host, _imapSettings.Port, _imapSettings.UseSsl, cancellationToken);

            await client.AuthenticateAsync(_imapSettings.UserName, _imapSettings.Password, cancellationToken);

            var drafts = client.GetFolder(SpecialFolder.Drafts);

            await drafts.OpenAsync(FolderAccess.ReadWrite, cancellationToken);

            await drafts.AppendAsync(new AppendRequest(message), cancellationToken);

            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
