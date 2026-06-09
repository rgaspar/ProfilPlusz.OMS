using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTOs.Email
{
    public class EmailDto
    {
        public string ExternalId { get; set; }
        public string Subject { get; set; }
        public string? Body { get; set; }
        public string? From { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
