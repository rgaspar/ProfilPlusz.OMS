using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Email.Models
{
    public class ProcessedEmail
    {
        public Guid Id { get; set; }

        public string ExternalId { get; set; }

        public string FromEmail { get; set; }

        public string Subject { get; set; }

        public string OrderNumber { get; set; }

        public string CustomerName { get; set; }

        public string County { get; set; }

        public bool PartnerExists { get; set; }

        public bool RecommendationSent { get; set; }

        public DateTime ReceivedAtUtc { get; set; }

        public DateTime ProcessedAtUtc { get; set; }

        public string Status { get; set; }

        public string ErrorMessage { get; set; }
    }
}
