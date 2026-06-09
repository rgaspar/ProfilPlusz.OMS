using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProcessedEmail : BaseEntity
    {
        public string ExternalId { get; set; }

        public string FromEmail { get; set; }

        public string Subject { get; set; }

        public string OrderNumber { get; set; }

        public string CustomerName { get; set; }

        public string State { get; set; }

        public bool CustomerExists { get; set; }

        public bool RecommendationSent { get; set; }

        public DateTime ReceivedAt { get; set; }

        public DateTime ProcessedAt { get; set; }

        public EmailProcessingStatus Status { get; set; }

        public string? ErrorMessage { get; set; }
    }
}