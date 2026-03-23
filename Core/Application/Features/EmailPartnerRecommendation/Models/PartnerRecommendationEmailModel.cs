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

        public List<string> Counties { get; set; } = new();

        public List<PartnerEmailModel> Partners { get; set; } = new();
    }
}
