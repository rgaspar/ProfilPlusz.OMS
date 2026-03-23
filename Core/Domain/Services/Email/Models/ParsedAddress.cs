using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Email.Models
{
    public class ParsedAddress
    {
        public string? Name { get; set; }
        public string? Street { get; set; }
        public string? FloorDoor { get; set; }
        public string? City { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
    }
}
