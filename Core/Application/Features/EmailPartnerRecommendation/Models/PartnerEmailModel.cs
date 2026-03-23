using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.EmailPartnerRecommendation.Models
{
    public class PartnerEmailModel
    {
        public string Name { get; set; } = null!;

        public string Zip { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string ContactName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string EmailPurchaseOrder { get; set; } = null!;
    }
}
