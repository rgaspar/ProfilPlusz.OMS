using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EmailManager
{
    public class EmailServiceSettings
    {
        public ImapSettings Imap { get; set; } = new();
        public SmtpSettings Smtp { get; set; } = new();
    }
}
