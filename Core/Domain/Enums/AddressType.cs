using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum AddressType
    {
        Billing = 1,      // Számlázási cím
        Shipping = 2,     // Szállítási cím
        Mailing = 3,      // Levelezési cím
        Headquarters = 4, // Székhely
        Site = 5,         // Telephely
        Other = 99
    }
}
