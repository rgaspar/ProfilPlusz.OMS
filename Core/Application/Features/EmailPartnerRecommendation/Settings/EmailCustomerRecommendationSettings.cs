using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.EmailCustomerRecommendation.Settings
{
    public class EmailCustomerRecommendationSettings
    {
        public string DefaultToEmail { get; set; } = "";

        public string DefaultToName { get; set; } = "";

        public string Subject { get; set; } = "";
    }
}
