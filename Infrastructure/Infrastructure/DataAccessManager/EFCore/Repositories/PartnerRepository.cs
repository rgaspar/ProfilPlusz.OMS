using Application.Common.Repositories;
using Domain.Entities;

namespace Infrastructure.DataAccessManager.EFCore.Repositories
{
    public class PartnerRepository : IPartnerRepository
    {
        public PartnerRepository()
        {

        }

        public Task<List<Partner>> GetByCountiesAsync(IEnumerable<string> counties, CancellationToken cancellationToken)
        {
            var partners = new List<Partner>
            {
                new Partner
                {
                    CustomerCode = "00000001",
                    Category1 = "Vevő",
                    Category2 = "Számlázási cím",
                    Name = "Hofstädter Építőanyag Centrum Kft.",
                    TaxNumber = "12997644-2-07",
                    EuTaxNumber = null,
                    ContactName = "Potári Petra",
                    EmailOrderConfirmation = "potari.petra@htuzep.hu",
                    EmailInvoice = "potari.petra@htuzep.hu",
                    EmailPurchaseOrder = "potari.petra@htuzep.hu",
                    Phone = "+36 20 312 1327",
                    Country = "Magyarország",
                    ZipCode = "2083",
                    City = "Solymár",
                    Address = "Valkó út 8",
                    County = "Pest vármegye",
                    BankAccount = "11780809-20000514",
                    InvoiceType = "papír",
                    PaymentMethod = "átutalás",
                    PaymentDeadline = "8 nap",
                    Currency = "HUF"
                },
                new Partner
                {
                    CustomerCode = "00000002",
                    Category1 = "Vevő",
                    Category2 = "Számlázási cím",
                    Name = "SIÓHÁZ CENTRUM Kft.",
                    TaxNumber = "29316304-2-13",
                    ContactName = "Kaszás Lajos",
                    EmailOrderConfirmation = "lazar@siohazcentrum.hu",
                    EmailInvoice = "lazar@siohazcentrum.hu",
                    EmailPurchaseOrder = "rendeles@siohazcentrum.hu",
                    Phone = "+36 20 310 8782",
                    Country = "Magyarország",
                    ZipCode = "8600",
                    City = "Siófok",
                    Address = "Ipar utca 2.",
                    County = "Somogy vármegye",
                    BankAccount = "10101315-64265400-01004000",
                    InvoiceType = "papír",
                    PaymentMethod = "átutalás",
                    PaymentDeadline = "15 nap",
                    Currency = "HUF"
                },
                new Partner
                {
                    CustomerCode = "00000003",
                    Category1 = "Beszállító",
                    Category2 = "Számlázási cím",
                    Name = "SIÓHÁZ CENTRUM Kft.",
                    TaxNumber = "29316304-2-13",
                    ContactName = "Kaszás Lajos",
                    EmailOrderConfirmation = "lajos@siohazcentrum.hu",
                    EmailInvoice = "lajos@siohazcentrum.hu",
                    EmailPurchaseOrder = "lajos@siohazcentrum.hu",
                    Phone = "+36 20 365 6467",
                    Country = "Magyarország",
                    ZipCode = "8600",
                    City = "Siófok",
                    Address = "Ipar utca 2.",
                    County = "Somogy vármegye",
                    BankAccount = "10101315-64265400-01004000",
                    InvoiceType = "papír",
                    PaymentMethod = "átutalás",
                    PaymentDeadline = "8 nap",
                    Currency = "HUF"
                },
                new Partner
                {
                    CustomerCode = "00000004",
                    Category1 = "Beszállító",
                    Category2 = "Számlázási cím",
                    Name = "Progress Profiles S.p.a.",
                    TaxNumber = null,
                    EuTaxNumber = "IT03481880262",
                    ContactName = "Benedetta Marchesan",
                    EmailOrderConfirmation = "bmarchesan@progressprofiles.com",
                    EmailInvoice = "bmarchesan@progressprofiles.com",
                    EmailPurchaseOrder = "bmarchesan@progressprofiles.com",
                    Phone = "+39 0423 950398",
                    Country = "Olaszország",
                    ZipCode = "31011",
                    City = "Asolo",
                    Address = "Via Le Marze, 7",
                    InvoiceType = "papír",
                    PaymentMethod = "átutalás",
                    PaymentDeadline = "30 nap",
                    Currency = "EUR"
                },
                new Partner
                {
                    CustomerCode = "00000005",
                    Category1 = "Vevő",
                    Category2 = "Számlázási cím",
                    Name = "MERKBAU Zrt.",
                    TaxNumber = "11430102-2-04",
                    ContactName = "Berta Evelin",
                    EmailOrderConfirmation = "csempehalas@merkbau.hu",
                    EmailInvoice = "szamla@merkbau.hu",
                    EmailPurchaseOrder = "berta.evelin@merkbau.hu",
                    Phone = "+36 30 345 2743",
                    Country = "Magyarország",
                    ZipCode = "6400",
                    City = "Kiskunhalas",
                    Address = "Jókai u. 75-79",
                    County = "Bács-Kiskun vármegye",
                    BankAccount = "10300002-90000000-00734528",
                    InvoiceType = "papír",
                    PaymentMethod = "átutalás",
                    PaymentDeadline = "8 nap",
                    Currency = "HUF"
                }
            };

            var normalizedCounties = counties.Select(NormalizeCounty).ToHashSet();

            var result = partners
                .Where(p =>
                    !string.IsNullOrWhiteSpace(p.County) &&
                    normalizedCounties.Contains(NormalizeCounty(p.County)))
                .ToList();

            return Task.FromResult(result);
        }

        private static string NormalizeCounty(string county)
        {
            return county
                .Replace(" vármegye", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" megye", "", StringComparison.OrdinalIgnoreCase)
                .Trim()
                .ToLowerInvariant();
        }
    }
}
