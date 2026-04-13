using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.EmailPartnerRecommendation.Models
{
    public class PartnerRecommendationEmailModel
    {
        public string CustomerName { get; set; } = null!;

        public List<string> States { get; set; } = new();

        public List<PartnerEmailModel> Customers { get; set; } = new();
    }
}
