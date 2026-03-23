using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EmailManager
{
    public class ImapSettings
    {
        public string? Host { get; set; }
        public int Port { get; set; }

        public bool UseSsl { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }

        public string[]? SubjectFilters { get; set; }
    }
}
