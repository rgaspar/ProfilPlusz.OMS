using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Email.Models
{
    public class ParsedProduct
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Quantity { get; set; }
    }
}
