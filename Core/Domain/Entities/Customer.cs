using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;


public class Customer : BaseEntity
{
    public string? Name { get; set; }
    public string? Number { get; set; }
    public string? Description { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FaxNumber { get; set; }

    public string? EmailAddress { get; set; }                                   //Default
    public string? EmailAddressOrderConfirmation { get; set; }                 //Rendelés visszaigazolás
    public string? EmailAddressInvoice { get; set; }                           //Számlázási
    public string? EmailAddressPurchaseOrder { get; set; }                     //Beszerzési

    public string? Website { get; set; }
    public string? WhatsApp { get; set; }
    public string? LinkedIn { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? TwitterX { get; set; }
    public string? TikTok { get; set; }

    public string? ContactPersonName { get; set; }

    public string? TaxNumber { get; set; }
    public string? EuTaxNumber { get; set; }
    public string? BankAccountNumber { get; set; }
    public InvoiceType? InvoiceType { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }

    public int? PaymentDeadlineDays { get; set; }
    public Currency? Currency { get; set; }


    public string? CustomerGroupId { get; set; }
    public CustomerGroup? CustomerGroup { get; set; }

    public string? CustomerCategoryId { get; set; }
    public CustomerCategory? CustomerCategory { get; set; }

    public ICollection<CustomerContact> CustomerContactList { get; set; } = new List<CustomerContact>();

    public ICollection<Address> AddressList { get; set; } = new List<Address>();
}
