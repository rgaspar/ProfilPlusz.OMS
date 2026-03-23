using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Email.Models
{
    public class ParsedEmail
    {
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }

        public ParsedAddress ShippingAddress { get; set; }

        public List<ParsedProduct> Products { get; set; } = new();
    }
}
